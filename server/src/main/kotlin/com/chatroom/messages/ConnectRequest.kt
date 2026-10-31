package com.chatroom.messages

import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
data class ConnectRequest(
    @SerialName("NickName")
    val nickName: String,
)
