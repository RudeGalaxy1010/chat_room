package com.chatroom.logic

private const val MIN_SPAWN_DISTANCE = 1.5f
private const val MAX_PLAYERS = 10

private data class SpawnPosition(
    val x: Float,
    val y: Float,
)

class Room {
    private val players = mutableMapOf<ClientSession, RoomPlayer>()
    private var playerIds = 0

    private val colors = listOf(
        "#E74C3C",
        "#2ECC71",
        "#3498DB",
        "#F1C40F",
        "#9B59B6",
        "#E67E22",
        "#1ABC9C",
        "#E84393",
        "#34495E",
        "#95A5A6",
    )

    private val spawnPositions = listOf(
        SpawnPosition(0f, 0f),
        SpawnPosition(-2f, 0f),
        SpawnPosition(2f, 0f),
        SpawnPosition(0f, -2f),
        SpawnPosition(0f, 2f),
        SpawnPosition(-2f, -2f),
        SpawnPosition(2f, -2f),
        SpawnPosition(-2f, 2f),
        SpawnPosition(2f, 2f),
        SpawnPosition(0f, 4f),
    )

    @Synchronized
    fun add(session: ClientSession, name: String): PlayerSnapshot? {
        if (
            players.size >= MAX_PLAYERS ||
            name.isBlank() ||
            players.values.any { it.name == name } ||
            players.containsKey(session)
        ) {
            return null
        }

        val spawnPosition = spawnPositions.firstOrNull { candidate ->
            players.values.none { player ->
                candidate.distanceSquaredTo(player) < MIN_SPAWN_DISTANCE * MIN_SPAWN_DISTANCE
            }
        } ?: return null
        val id = ++playerIds
        val color = colors.firstOrNull { candidate ->
            players.values.none { it.color == candidate }
        } ?: colors[(id - 1) % colors.size]
        val player = RoomPlayer(
            session = session,
            id = id,
            x = spawnPosition.x,
            y = spawnPosition.y,
            name = name,
            color = color,
        )

        players[session] = player
        return player.snapshot()
    }

    @Synchronized
    fun markReady(session: ClientSession): ReadyResult? {
        val player = players[session] ?: return null

        if (player.isReady) {
            return null
        }

        val existingPlayers = players.values
            .filter { it.isReady }
            .map(RoomPlayer::snapshot)
        player.isReady = true
        return ReadyResult(player.snapshot(), existingPlayers)
    }

    @Synchronized
    fun updatePosition(session: ClientSession, playerId: Int, x: Float, y: Float): PlayerSnapshot? {
        val player = players[session] ?: return null
        if (!player.isReady || player.id != playerId) {
            return null
        }

        player.x = x
        player.y = y
        return player.snapshot()
    }

    @Synchronized
    fun readyPlayer(session: ClientSession, playerId: Int): PlayerSnapshot? {
        val player = players[session] ?: return null
        return player.takeIf { it.isReady && it.id == playerId }?.snapshot()
    }

    @Synchronized
    fun readyPlayers(): List<PlayerSnapshot> = players.values
        .filter { it.isReady }
        .map(RoomPlayer::snapshot)

    @Synchronized
    fun remove(session: ClientSession): LeaveResult? {
        val player = players.remove(session) ?: return null

        if (!player.isReady) {
            return null
        }

        return LeaveResult(
            player = player.snapshot(),
            remainingPlayers = players.values
                .filter { it.isReady }
                .map(RoomPlayer::snapshot),
        )
    }
}

private fun SpawnPosition.distanceSquaredTo(player: RoomPlayer): Float {
    val deltaX = x - player.x
    val deltaY = y - player.y
    return deltaX * deltaX + deltaY * deltaY
}
