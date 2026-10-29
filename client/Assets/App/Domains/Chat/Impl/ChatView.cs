using System.Collections.Generic;
using App.Domains.Chat.Api;
using UnityEngine;

namespace App.Domains.Chat.Impl
{
    public class ChatView : View, IChatView
    {
        [SerializeField] private ChatMessageView _chatMessageViewPrefab = null!;
        [SerializeField] private ChatInputView _inputView = null!;
        [SerializeField] private Transform _messagesRoot = null!;

        private readonly Dictionary<int, IChatMessageView> _messages = new Dictionary<int, IChatMessageView>();

        public IChatInputView ChatInputView => _inputView;

        public void AddMessage(int id, string nickName, string message)
        {
            IChatMessageView messageView = Instantiate(_chatMessageViewPrefab, _messagesRoot);
            messageView.SetInfo(nickName, message);
            _messages[id] = messageView;
        }

        public void RemoveMessage(int id)
        {
            if (_messages.TryGetValue(id, out IChatMessageView messageView))
            {
                messageView.Despawn();
                _messages.Remove(id);
            }
        }
    }
}