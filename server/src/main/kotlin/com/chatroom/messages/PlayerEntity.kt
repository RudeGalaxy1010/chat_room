package com.chatroom.messages

import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
data class PlayerEntity(
    @SerialName("Id")
    val id: Int,
    @SerialName("X")
    val x: Float,
    @SerialName("Y")
    val y: Float,
    @SerialName("Name")
    val name: String,
    @SerialName("Color")
    val color: String,
)
