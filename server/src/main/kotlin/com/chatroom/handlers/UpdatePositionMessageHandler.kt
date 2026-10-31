package com.chatroom.handlers

import com.chatroom.logic.ClientSession
import com.chatroom.logic.OutgoingMessage
import com.chatroom.logic.Room
import com.chatroom.messages.MessageStatus
import com.chatroom.messages.MessageType
import com.chatroom.messages.MessageWrapper
import com.chatroom.messages.UpdatePositionRequest
import com.chatroom.messages.UpdatePositionResponse
import kotlinx.serialization.json.Json

class UpdatePositionMessageHandler(
    private val json: Json,
    private val room: Room,
) : MessageHandler {
    override fun handle(session: ClientSession, message: MessageWrapper): List<OutgoingMessage> {
        val request = json.decodeFromString<UpdatePositionRequest>(message.data)

        if (!request.x.isFinite() || !request.y.isFinite()) {
            return emptyList()
        }

        val player = room.updatePosition(session, request.id, request.x, request.y)
            ?: return emptyList()
        val response = UpdatePositionResponse(
            status = MessageStatus.OK.value,
            id = player.entity.id,
            x = player.entity.x,
            y = player.entity.y,
        )
        val responseMessage = MessageWrapper(
            type = MessageType.UPDATE_POSITION.value,
            data = json.encodeToString(response),
        )

        return room.readyPlayers().map { target ->
            OutgoingMessage(target.session, responseMessage)
        }
    }
}
