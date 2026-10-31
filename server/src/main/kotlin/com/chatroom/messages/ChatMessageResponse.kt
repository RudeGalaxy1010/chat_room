package com.chatroom.messages

import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
data class ChatMessageResponse(
    @SerialName("Status")
    val status: Int,
    @SerialName("Id")
    val id: Int,
    @SerialName("NickName")
    val nickName: String,
    @SerialName("Message")
    val message: String,
    @SerialName("Color")
    val color: String,
)
