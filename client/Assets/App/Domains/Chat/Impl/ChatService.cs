using System;
using App.Domains.Chat.Api;
using App.Domains.Core.Api;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;

namespace App.Domains.Chat.Impl
{
    public class ChatService : IChatService, IDisposable
    {
        private readonly IMessageService _messageService;
        private readonly ILocalContext _localContext;
        private readonly IChatView _chatView;

        public ChatService(IMessageService messageService,
            ILocalContext localContext,
            IChatView chatView)
        {
            _messageService = messageService;
            _localContext = localContext;
            _chatView = chatView;

            _chatView.ChatInputView.OnSubmit += SendMessage;
            _messageService.Register(MessageType.ChatMessage, OnChatMessageReceived);
        }

        public void Dispose()
        {
            _messageService.Unregister(MessageType.ChatMessage, OnChatMessageReceived);
            _chatView.ChatInputView.OnSubmit -= SendMessage;
        }

        private void OnChatMessageReceived(Response response)
        {
            ChatMessageResponse chatMessageResponse = (ChatMessageResponse)response;

            if (chatMessageResponse.IsOk)
            {
                _chatView.AddMessage(chatMessageResponse.NickName,
                    chatMessageResponse.Message);
            }
        }

        private void SendMessage(string value)
        {
            if (value == string.Empty)
            {
                return;
            }

            _chatView.ChatInputView.Clear();

            _messageService.Send(new ChatMessageRequest
            {
                Id = _localContext.Id,
                Message = value,
            });
        }
    }
}