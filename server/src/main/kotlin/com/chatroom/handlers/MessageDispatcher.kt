package com.chatroom.handlers

import com.chatroom.logic.ClientSession
import com.chatroom.logic.OutgoingMessage
import com.chatroom.logic.Room
import com.chatroom.messages.ConnectResponse
import com.chatroom.messages.LeaveResponse
import com.chatroom.messages.MessageStatus
import com.chatroom.messages.MessageType
import com.chatroom.messages.MessageWrapper
import kotlinx.serialization.SerializationException
import kotlinx.serialization.json.Json
import org.slf4j.LoggerFactory

class MessageDispatcher(
    private val json: Json = Json { ignoreUnknownKeys = true },
    private val room: Room = Room(),
) {
    private val logger = LoggerFactory.getLogger(MessageDispatcher::class.java)

    private val handlers: Map<MessageType, MessageHandler> = mapOf(
        MessageType.CONNECT to ConnectMessageHandler(json, room),
        MessageType.READY to ReadyMessageHandler(json, room),
        MessageType.UPDATE_POSITION to UpdatePositionMessageHandler(json, room),
        MessageType.CHAT_MESSAGE to ChatMessageMessageHandler(json, room),
    )

    fun handle(session: ClientSession, rawMessage: String): List<OutgoingMessage> {
        val message = parseMessage(rawMessage) ?: return emptyList()
        val messageType = MessageType.from(message.type)

        if (messageType == null) {
            logger.warn("Unknown message type: {}", message.type)
            return emptyList()
        }

        try {
            val handler = handlers[messageType]
            if (handler == null) {
                logger.warn("No handler registered for message type: {}", messageType)
                return emptyList()
            }

            return handler.handle(session, message)
        } catch (exception: SerializationException) {
            logger.warn("Invalid {} message", messageType, exception)
            return if (messageType == MessageType.CONNECT) {
                connectFailure(session)
            } else {
                emptyList()
            }
        } catch (exception: Exception) {
            logger.error("Failed to handle {} message", messageType, exception)
            throw exception
        }
    }

    fun disconnect(session: ClientSession): List<OutgoingMessage> {
        val leaveResult = room.remove(session) ?: return emptyList()
        val response = LeaveResponse(
            status = MessageStatus.OK.value,
            id = leaveResult.player.entity.id,
        )
        val responseMessage = MessageWrapper(
            type = MessageType.LEAVE.value,
            data = json.encodeToString(response),
        )

        return leaveResult.remainingPlayers.map { player ->
            OutgoingMessage(player.session, responseMessage)
        }
    }

    private fun connectFailure(session: ClientSession): List<OutgoingMessage> {
        val response = ConnectResponse(
            status = MessageStatus.EXCEPTION.value,
            id = -1,
            name = "",
        )
        return listOf(
            OutgoingMessage(
                target = session,
                message = MessageWrapper(
                    type = MessageType.CONNECT.value,
                    data = json.encodeToString(response),
                ),
            ),
        )
    }

    private fun parseMessage(rawMessage: String): MessageWrapper? {
        val typeSeparatorIndex = rawMessage.indexOf('-')

        if (typeSeparatorIndex <= 0) {
            logger.warn("Invalid message format")
            return null
        }

        val type = rawMessage.substring(0, typeSeparatorIndex).toIntOrNull()

        if (type == null) {
            logger.warn("Invalid message type")
            return null
        }

        return MessageWrapper(
            type = type,
            data = rawMessage.substring(typeSeparatorIndex + 1),
        )
    }
}
