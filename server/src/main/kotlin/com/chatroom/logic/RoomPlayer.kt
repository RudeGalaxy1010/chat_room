package com.chatroom.logic

import com.chatroom.messages.PlayerEntity

class RoomPlayer(
    val session: ClientSession,
    val id: Int,
    var x: Float,
    var y: Float,
    val name: String,
    val color: String,
    var isReady: Boolean = false,
) {
    fun snapshot() = PlayerSnapshot(
        session = session,
        entity = PlayerEntity(
            id = id,
            x = x,
            y = y,
            name = name,
            color = color,
        ),
    )
}
