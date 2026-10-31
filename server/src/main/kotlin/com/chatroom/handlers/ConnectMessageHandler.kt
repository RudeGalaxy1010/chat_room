package com.chatroom.handlers

import com.chatroom.logic.ClientSession
import com.chatroom.logic.OutgoingMessage
import com.chatroom.logic.Room
import com.chatroom.messages.ConnectRequest
import com.chatroom.messages.ConnectResponse
import com.chatroom.messages.MessageStatus
import com.chatroom.messages.MessageType
import com.chatroom.messages.MessageWrapper
import kotlinx.serialization.json.Json

class ConnectMessageHandler(
    private val json: Json,
    private val room: Room,
) : MessageHandler {
    override fun handle(session: ClientSession, message: MessageWrapper): List<OutgoingMessage> {
        val request = json.decodeFromString<ConnectRequest>(message.data)
        val player = room.add(session, request.nickName)
        val response = if (player == null) {
            ConnectResponse(
                status = MessageStatus.EXCEPTION.value,
                id = -1,
                name = "",
            )
        } else {
            ConnectResponse(
                status = MessageStatus.OK.value,
                id = player.entity.id,
                name = player.entity.name,
            )
        }

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
}
