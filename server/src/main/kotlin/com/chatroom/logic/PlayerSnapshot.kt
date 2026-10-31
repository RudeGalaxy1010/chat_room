package com.chatroom.logic

import com.chatroom.messages.PlayerEntity

data class PlayerSnapshot(
    val session: ClientSession,
    val entity: PlayerEntity,
)
