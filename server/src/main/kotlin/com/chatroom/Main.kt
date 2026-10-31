package com.chatroom

import com.chatroom.handlers.MessageDispatcher
import com.chatroom.transport.ConnectionHub
import io.ktor.server.application.*
import io.ktor.server.engine.*
import io.ktor.server.netty.*
import io.ktor.server.routing.*
import io.ktor.server.websocket.*
import io.ktor.websocket.*
import kotlin.time.Duration.Companion.seconds

fun main() {
    val connections = ConnectionHub(MessageDispatcher())

    embeddedServer(Netty, host = "0.0.0.0", port = 5000) {
        install(WebSockets) {
            pingPeriod = 15.seconds
            timeout = 45.seconds
        }

        routing {
            webSocket("/") {
                val session = connections.connect(this, this)

                try {
                    for (frame in incoming) {
                        if (frame is Frame.Text) {
                            connections.handle(session, frame.readText())
                        }
                    }
                } finally {
                    connections.disconnect(session)
                }
            }
        }
    }.start(wait = true)
}
