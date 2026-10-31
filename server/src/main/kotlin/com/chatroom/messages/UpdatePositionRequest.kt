package com.chatroom.messages

import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
data class UpdatePositionRequest(
    @SerialName("Id")
    val id: Int,
    @SerialName("X")
    val x: Float,
    @SerialName("Y")
    val y: Float,
)
