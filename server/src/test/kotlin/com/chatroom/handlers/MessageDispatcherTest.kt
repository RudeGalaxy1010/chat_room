package com.chatroom.handlers

import com.chatroom.logic.ClientSession
import com.chatroom.messages.ConnectResponse
import com.chatroom.messages.MessageStatus
import com.chatroom.messages.MessageType
import kotlinx.serialization.json.Json
import kotlin.test.Test
import kotlin.test.assertEquals

class MessageDispatcherTest {
    @Test
    fun `malformed connect receives a failure response`() {
        val response = MessageDispatcher().handle(ClientSession(), "0-{}")

        assertEquals(1, response.size)
        assertEquals(MessageType.CONNECT.value, response.single().message.type)

        val connectResponse = Json.decodeFromString<ConnectResponse>(response.single().message.data)
        assertEquals(MessageStatus.EXCEPTION.value, connectResponse.status)
        assertEquals(-1, connectResponse.id)
    }
}
