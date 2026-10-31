package com.chatroom.handlers

import com.chatroom.logic.ClientSession
import com.chatroom.logic.OutgoingMessage
import com.chatroom.messages.MessageWrapper

interface MessageHandler {
    fun handle(session: ClientSession, message: MessageWrapper): List<OutgoingMessage>
}
