package com.chatroom.messages

import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
data class JoinResponse(
    @SerialName("Status")
    val status: Int,
    @SerialName("Player")
    val player: PlayerEntity,
)
