package com.chatroom.messages

import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
data class LeaveResponse(
    @SerialName("Status")
    val status: Int,
    @SerialName("Id")
    val id: Int,
)
