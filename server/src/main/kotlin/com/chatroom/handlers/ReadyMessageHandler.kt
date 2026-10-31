package com.chatroom.handlers

import com.chatroom.logic.ClientSession
import com.chatroom.logic.OutgoingMessage
import com.chatroom.logic.ReadyResult
import com.chatroom.logic.Room
import com.chatroom.messages.JoinResponse
import com.chatroom.messages.MessageStatus
import com.chatroom.messages.MessageType
import com.chatroom.messages.MessageWrapper
import com.chatroom.messages.PlayerEntity
import com.chatroom.messages.ReadyRequest
import kotlinx.serialization.json.Json

class ReadyMessageHandler(
    private val json: Json,
    private val room: Room,
) : MessageHandler {
    override fun handle(session: ClientSession, message: MessageWrapper): List<OutgoingMessage> {
        json.decodeFromString<ReadyRequest>(message.data)

        val readyResult = room.markReady(session) ?: return emptyList()
        return createJoinMessages(readyResult)
    }

    private fun createJoinMessages(result: ReadyResult): List<OutgoingMessage> {
        val newPlayerMessage = createJoinMessage(result.player.entity)
        val messages = result.existingPlayers
            .map { player ->
                OutgoingMessage(
                    target = result.player.session,
                    message = createJoinMessage(player.entity),
                )
            }
            .toMutableList()

        messages += OutgoingMessage(
            target = result.player.session,
            message = newPlayerMessage,
        )

        result.existingPlayers.forEach { player ->
            messages += OutgoingMessage(
                target = player.session,
                message = newPlayerMessage,
            )
        }

        return messages
    }

    private fun createJoinMessage(player: PlayerEntity): MessageWrapper {
        val response = JoinResponse(
            status = MessageStatus.OK.value,
            player = player,
        )

        return MessageWrapper(
            type = MessageType.JOIN.value,
            data = json.encodeToString(response),
        )
    }
}
