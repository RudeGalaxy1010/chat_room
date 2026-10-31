package com.chatroom.logic

data class LeaveResult(
    val player: PlayerSnapshot,
    val remainingPlayers: List<PlayerSnapshot>,
)
