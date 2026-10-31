package com.chatroom.messages

import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
data class ChatMessageRequest(
    @SerialName("Id")
    val id: Int,
    @SerialName("Message")
    val message: String,
)
