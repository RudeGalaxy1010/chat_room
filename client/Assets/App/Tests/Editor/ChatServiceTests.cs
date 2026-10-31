using System;
using App.Domains.Chat.Api;
using App.Domains.Chat.Impl;
using App.Domains.Core.Api;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using App.Domains.UIManagement.Api;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace App.Tests.Domains.Chat
{
    public class ChatServiceTests
    {
        [Test]
        public void SubmitMessage_WhenTextIsNotBlank_SendsMessageAndClearsInput()
        {
            // Arrange
            IMessageService messageService = Substitute.For<IMessageService>();
            ILocalContext localContext = Substitute.For<ILocalContext>();
            IChatView chatView = Substitute.For<IChatView>();
            ITextInputView inputView = Substitute.For<ITextInputView>();
            localContext.Id.Returns(7);
            chatView.ChatInputView.Returns(inputView);
            using ChatService service = Create(messageService, localContext, chatView);

            // Act
            inputView.OnSubmit += Raise.Event<Action<string>>("Hello");

            // Assert
            inputView.Received(1).Clear();
            messageService.Received(1).Send(Arg.Is<ChatMessageRequest>(message =>
                message.Id == 7 && message.Message == "Hello"));
        }

        [Test]
        public void SubmitMessage_WhenTextIsBlank_DoesNotSendMessage()
        {
            // Arrange
            IMessageService messageService = Substitute.For<IMessageService>();
            ILocalContext localContext = Substitute.For<ILocalContext>();
            IChatView chatView = Substitute.For<IChatView>();
            ITextInputView inputView = Substitute.For<ITextInputView>();
            chatView.ChatInputView.Returns(inputView);
            using ChatService service = Create(messageService, localContext, chatView);

            // Act
            inputView.OnSubmit += Raise.Event<Action<string>>("   ");

            // Assert
            inputView.DidNotReceive().Clear();
            messageService.DidNotReceive().Send(Arg.Any<ChatMessageRequest>());
        }

        [Test]
        public void ReceiveMessage_WhenStatusIsOk_AddsMessageToView()
        {
            // Arrange
            IMessageService messageService = Substitute.For<IMessageService>();
            ILocalContext localContext = Substitute.For<ILocalContext>();
            IChatView chatView = Substitute.For<IChatView>();
            ITextInputView inputView = Substitute.For<ITextInputView>();
            Action<Response> chatHandler = null;
            chatView.ChatInputView.Returns(inputView);
            messageService.Register(Arg.Any<MessageType>(), Arg.Do<Action<Response>>(
                handler => chatHandler = handler));
            using ChatService service = Create(messageService, localContext, chatView);

            // Act
            chatHandler(new ChatMessageResponse
            {
                Status = (int)MessageStatus.Ok,
                NickName = "Alice",
                Message = "Hello",
                Color = "#E74C3C",
            });

            // Assert
            chatView.Received(1).AddMessage(
                "Alice",
                Arg.Any<Color>(),
                "Hello");
        }

        private static ChatService Create(
            IMessageService messageService = null,
            ILocalContext localContext = null,
            IChatView chatView = null)
        {
            return new ChatService(
                messageService ?? Substitute.For<IMessageService>(),
                localContext ?? Substitute.For<ILocalContext>(),
                chatView ?? Substitute.For<IChatView>());
        }
    }
}
