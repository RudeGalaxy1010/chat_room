package com.chatroom.logic

import kotlin.test.Test
import kotlin.test.assertEquals
import kotlin.test.assertNotNull
import kotlin.test.assertNull
import kotlin.test.assertNotEquals

class RoomTest {
    @Test
    fun `position update does not mutate an existing snapshot`() {
        val room = Room()
        val session = ClientSession()
        val player = room.add(session, "Alice")
        assertNotNull(player)

        assertNotNull(room.markReady(session))
        val initialSnapshot = room.readyPlayer(session, player.entity.id)
        assertNotNull(initialSnapshot)

        val updatedSnapshot = room.updatePosition(session, player.entity.id, 10f, 20f)
        assertNotNull(updatedSnapshot)
        assertEquals(10f, updatedSnapshot.entity.x)
        assertEquals(20f, updatedSnapshot.entity.y)

        val currentSnapshot = room.readyPlayer(session, player.entity.id)
        assertNotNull(currentSnapshot)
        assertEquals(10f, currentSnapshot.entity.x)
        assertEquals(20f, currentSnapshot.entity.y)

        assertEquals(0f, initialSnapshot.entity.x)
        assertEquals(0f, initialSnapshot.entity.y)
    }

    @Test
    fun `room accepts at most ten players`() {
        val room = Room()

        repeat(10) { index ->
            assertNotNull(room.add(ClientSession(), "Player$index"))
        }

        assertNull(room.add(ClientSession(), "Player11"))
    }

    @Test
    fun `new player receives a color not used by active players`() {
        val room = Room()
        val firstPlayer = room.add(ClientSession(), "Player1")
        assertNotNull(firstPlayer)
        val otherSessions = (2..10).map { index ->
            val session = ClientSession()
            assertNotNull(room.add(session, "Player$index"))
            session
        }

        otherSessions.forEach(room::remove)
        val replacement = room.add(ClientSession(), "Replacement")
        assertNotNull(replacement)

        assertNotEquals(firstPlayer.entity.color, replacement.entity.color)
    }
}
