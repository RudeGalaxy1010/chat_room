package com.chatroom.logic

data class ReadyResult(
    val player: PlayerSnapshot,
    val existingPlayers: List<PlayerSnapshot>,
)
