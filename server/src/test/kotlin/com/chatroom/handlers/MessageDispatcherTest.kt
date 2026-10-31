package com.chatroom.handlers

import com.chatroom.logic.ClientSession
import com.chatroom.messages.ConnectResponse
import com.chatroom.messages.LeaveResponse
import com.chatroom.messages.MessageStatus
import com.chatroom.messages.MessageType
import com.chatroom.messages.UpdatePositionResponse
import kotlinx.serialization.json.Json
import kotlin.test.Test
import kotlin.test.assertEquals
import kotlin.test.assertTrue

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

    @Test
    fun `player cannot update another session position`() {
        val dispatcher = MessageDispatcher()
        val firstSession = ClientSession()
        val secondSession = ClientSession()
        val firstId = connect(dispatcher, firstSession, "Alice")
        val secondId = connect(dispatcher, secondSession, "Bob")

        dispatcher.handle(firstSession, "1-{}")
        dispatcher.handle(secondSession, "1-{}")

        val responses = dispatcher.handle(
            secondSession,
            "3-{\"Id\":$firstId,\"X\":10.0,\"Y\":20.0}",
        )

        assertTrue(responses.isEmpty())
        val validResponses = dispatcher.handle(
            secondSession,
            "3-{\"Id\":$secondId,\"X\":5.0,\"Y\":6.0}",
        )
        val position = Json.decodeFromString<UpdatePositionResponse>(
            validResponses.first().message.data,
        )
        assertEquals(5.0f, position.x)
        assertEquals(6.0f, position.y)
    }

    @Test
    fun `ready player receives join and leave lifecycle messages`() {
        val dispatcher = MessageDispatcher()
        val firstSession = ClientSession()
        val secondSession = ClientSession()
        val firstId = connect(dispatcher, firstSession, "Alice")

        dispatcher.handle(firstSession, "1-{}")
        val secondId = connect(dispatcher, secondSession, "Bob")
        val secondReadyResponses = dispatcher.handle(secondSession, "1-{}")

        assertEquals(3, secondReadyResponses.size)
        assertEquals(2, dispatcher.handle(firstSession, "3-{\"Id\":$firstId,\"X\":1.0,\"Y\":2.0}").size)

        val leaveResponses = dispatcher.disconnect(secondSession)
        assertEquals(1, leaveResponses.size)
        val leaveResponse = Json.decodeFromString<LeaveResponse>(
            leaveResponses.single().message.data,
        )
        assertEquals(MessageType.LEAVE.value, leaveResponses.single().message.type)
        assertEquals(secondId, leaveResponse.id)
    }

    private fun connect(dispatcher: MessageDispatcher, session: ClientSession, name: String): Int {
        val response = dispatcher.handle(session, "0-{\"NickName\":\"$name\"}").single()
        return Json.decodeFromString<ConnectResponse>(response.message.data).id
    }
}
