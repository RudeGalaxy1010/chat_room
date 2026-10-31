using App.Domains.Chat.Api;
using App.Domains.UIManagement.Api;
using App.Domains.UIManagement.Impl;
using UnityEngine;

namespace App.Domains.Chat.Impl
{
    public class ChatView : View, IChatView
    {
        [SerializeField] private ChatMessageView _chatMessageViewPrefab = null!;
        [SerializeField] private TextInputView _inputView = null!;
        [SerializeField] private Transform _messagesRoot = null!;

        public ITextInputView ChatInputView => _inputView;

        public void AddMessage(string nickName, Color nickNameColor, string message)
        {
            IChatMessageView messageView = Instantiate(_chatMessageViewPrefab, _messagesRoot);
            messageView.SetInfo(nickName, nickNameColor, message);
        }
    }
}