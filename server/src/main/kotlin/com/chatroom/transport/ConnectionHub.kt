package com.chatroom.transport

import com.chatroom.handlers.MessageDispatcher
import com.chatroom.logic.ClientSession
import com.chatroom.logic.OutgoingMessage
import io.ktor.server.websocket.DefaultWebSocketServerSession
import io.ktor.websocket.Frame
import kotlinx.coroutines.CancellationException
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Job
import kotlinx.coroutines.NonCancellable
import kotlinx.coroutines.cancel
import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.currentCoroutineContext
import kotlinx.coroutines.ensureActive
import kotlinx.coroutines.launch
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import kotlinx.coroutines.withContext
import org.slf4j.LoggerFactory
import java.util.concurrent.atomic.AtomicBoolean

class ConnectionHub(
    private val dispatcher: MessageDispatcher,
) {
    private val logger = LoggerFactory.getLogger(ConnectionHub::class.java)
    private val mutex = Mutex()
    private val connections = mutableMapOf<ClientSession, ClientConnection>()

    suspend fun connect(
        socket: DefaultWebSocketServerSession,
        scope: CoroutineScope,
    ): ClientSession {
        val session = ClientSession()
        val connection = ClientConnection(session, socket) { failedSession ->
            disconnect(failedSession)
        }

        mutex.withLock {
            connections[session] = connection
        }
        connection.start(scope)
        logger.info("Client connected")
        return session
    }

    suspend fun handle(session: ClientSession, rawMessage: String) {
        val failedSessions = mutex.withLock {
            if (session !in connections) {
                return@withLock emptySet()
            }

            val responses = dispatcher.handle(session, rawMessage)
            enqueue(responses)
        }

        for (failedSession in failedSessions) {
            disconnect(failedSession)
        }
    }

    suspend fun disconnect(session: ClientSession) {
        val connection = withContext(NonCancellable) {
            mutex.withLock {
                val removedConnection = connections.remove(session) ?: return@withLock null
                val leaveResponses = dispatcher.disconnect(session)
                val failedSessions = enqueue(leaveResponses)
                removedConnection to failedSessions
            }
        } ?: return

        val (clientConnection, failedSessions) = connection
        clientConnection.stop()
        for (failedSession in failedSessions) {
            disconnect(failedSession)
        }
        logger.info("Client disconnected")
    }

    private fun enqueue(messages: List<OutgoingMessage>): Set<ClientSession> {
        return messages.mapNotNull { message ->
            val connection = connections[message.target] ?: return@mapNotNull null
            if (connection.enqueue(message)) null else message.target
        }.toSet()
    }
}

private class ClientConnection(
    private val session: ClientSession,
    private val socket: DefaultWebSocketServerSession,
    private val onWriterFailure: suspend (ClientSession) -> Unit,
) {
    private val logger = LoggerFactory.getLogger(ClientConnection::class.java)
    private val outgoing = Channel<String>(Channel.UNLIMITED)
    private val stopped = AtomicBoolean(false)
    private var writerJob: Job? = null

    fun start(scope: CoroutineScope) {
        writerJob = scope.launch {
            try {
                for (message in outgoing) {
                    socket.send(Frame.Text(message))
                }
            } catch (exception: CancellationException) {
                currentCoroutineContext().ensureActive()
                logger.warn("WebSocket writer was cancelled unexpectedly", exception)
                onWriterFailure(session)
            } catch (exception: Exception) {
                logger.warn("Failed to send a message to client", exception)
                onWriterFailure(session)
            }
        }
    }

    fun enqueue(message: OutgoingMessage): Boolean {
        val wireMessage = "${message.message.type}-${message.message.data}"
        return outgoing.trySend(wireMessage).isSuccess
    }

    fun stop() {
        if (!stopped.compareAndSet(false, true)) {
            return
        }

        outgoing.close()
        writerJob?.cancel()
        socket.cancel()
    }
}
