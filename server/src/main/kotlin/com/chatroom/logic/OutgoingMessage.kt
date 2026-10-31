package com.chatroom.logic

import com.chatroom.messages.MessageWrapper

data class OutgoingMessage(
    val target: ClientSession,
    val message: MessageWrapper,
)
