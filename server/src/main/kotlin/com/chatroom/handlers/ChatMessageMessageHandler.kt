package com.chatroom.handlers

import com.chatroom.logic.ClientSession
import com.chatroom.logic.OutgoingMessage
import com.chatroom.logic.Room
import com.chatroom.messages.ChatMessageRequest
import com.chatroom.messages.ChatMessageResponse
import com.chatroom.messages.MessageStatus
import com.chatroom.messages.MessageType
import com.chatroom.messages.MessageWrapper
import kotlinx.serialization.json.Json

class ChatMessageMessageHandler(
    private val json: Json,
    private val room: Room,
) : MessageHandler {
    override fun handle(session: ClientSession, message: MessageWrapper): List<OutgoingMessage> {
        val request = json.decodeFromString<ChatMessageRequest>(message.data)

        if (request.message.isBlank()) {
            return emptyList()
        }

        val player = room.readyPlayer(session, request.id) ?: return emptyList()
        val response = ChatMessageResponse(
            status = MessageStatus.OK.value,
            id = player.entity.id,
            nickName = player.entity.name,
            message = request.message,
            color = player.entity.color,
        )
        val responseMessage = MessageWrapper(
            type = MessageType.CHAT_MESSAGE.value,
            data = json.encodeToString(response),
        )

        return room.readyPlayers().map { target ->
            OutgoingMessage(target.session, responseMessage)
        }
    }
}
