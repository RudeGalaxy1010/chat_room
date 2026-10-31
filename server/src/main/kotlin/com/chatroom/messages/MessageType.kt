package com.chatroom.messages

enum class MessageType(val value: Int) {
    CONNECT(0),
    READY(1),
    JOIN(2),
    UPDATE_POSITION(3),
    CHAT_MESSAGE(4),
    LEAVE(5),
    ;

    companion object {
        fun from(value: Int): MessageType? = entries.firstOrNull { it.value == value }
    }
}
