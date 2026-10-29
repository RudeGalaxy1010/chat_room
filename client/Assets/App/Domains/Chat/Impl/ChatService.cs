using System;
using App.Domains.Chat.Api;
using UnityEngine;

namespace App.Domains.Chat.Impl
{
    public class ChatService : IChatService, IDisposable
    {
        private const int TempMessageId = -1;

        private readonly IChatView _chatView;

        public ChatService(IChatView chatView)
        {
            _chatView = chatView;
            _chatView.ChatInputView.OnMessageSubmitted += SendMessage;
        }

        public void Dispose() =>
            _chatView.ChatInputView.OnMessageSubmitted -= SendMessage;

        private void SendMessage(string value)
        {
            _chatView.ChatInputView.Clear();
            // TODO: Send to server
            Debug.Log($"Message sent: {value}");
            _chatView.AddMessage(TempMessageId, "", value);
        }
    }
}