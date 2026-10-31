using System;
using App.Domains.Core.Api;
using App.Domains.Network.Api;
using App.Domains.Network.Impl;
using App.Domains.Network.Impl.Messages;
using NSubstitute;
using NUnit.Framework;
using R3;

namespace App.Tests.Domains.Network
{
    public class MessageServiceTests
    {
        [Test]
        public void Send_WhenConnected_SendsEncodedMessage()
        {
            // Arrange
            INetworkService networkService = Substitute.For<INetworkService>();
            IMessageEncoder messageEncoder = Substitute.For<IMessageEncoder>();
            ILogger logger = Substitute.For<ILogger>();
            ReactiveProperty<bool> isConnected = new ReactiveProperty<bool>(true);
            networkService.MessageQueue.Returns(new Subject<MessageWrapper>());
            networkService.IsConnected.Returns(isConnected);
            messageEncoder.Encode(Arg.Any<ConnectRequest>()).Returns("encoded");
            using MessageService service = Create(networkService, messageEncoder, logger);

            // Act
            service.Send(new ConnectRequest { NickName = "Alice" });

            // Assert
            networkService.Received(1).SendMessage(Arg.Is<MessageWrapper>(message =>
                message.Type == (int)MessageType.Connect && message.Data == "encoded"));
        }

        [Test]
        public void Send_WhenDisconnected_DoesNotSendMessage()
        {
            // Arrange
            INetworkService networkService = Substitute.For<INetworkService>();
            IMessageEncoder messageEncoder = Substitute.For<IMessageEncoder>();
            ILogger logger = Substitute.For<ILogger>();
            ReactiveProperty<bool> isConnected = new ReactiveProperty<bool>(false);
            networkService.MessageQueue.Returns(new Subject<MessageWrapper>());
            networkService.IsConnected.Returns(isConnected);
            using MessageService service = Create(networkService, messageEncoder, logger);

            // Act
            service.Send(new ReadyRequest());

            // Assert
            networkService.DidNotReceive().SendMessage(Arg.Any<MessageWrapper>());
            logger.Received(1).LogError("Network service is not connected");
        }

        [Test]
        public void ReceiveMessage_InvokesRegisteredHandler()
        {
            // Arrange
            INetworkService networkService = Substitute.For<INetworkService>();
            IMessageEncoder messageEncoder = Substitute.For<IMessageEncoder>();
            ILogger logger = Substitute.For<ILogger>();
            Subject<MessageWrapper> messageQueue = new Subject<MessageWrapper>();
            ReactiveProperty<bool> isConnected = new ReactiveProperty<bool>(true);
            networkService.MessageQueue.Returns(messageQueue);
            networkService.IsConnected.Returns(isConnected);
            messageEncoder.Decode<ConnectResponse>(Arg.Any<string>()).Returns(new ConnectResponse
            {
                Status = (int)MessageStatus.Ok,
            });
            using MessageService service = Create(networkService, messageEncoder, logger);
            int receivedCount = 0;

            service.Register(MessageType.Connect, _ => receivedCount++);

            // Act
            messageQueue.OnNext(new MessageWrapper
            {
                Type = (int)MessageType.Connect,
                Data = "{}",
            });

            // Assert
            Assert.That(receivedCount, Is.EqualTo(1));
        }

        private static MessageService Create(
            INetworkService networkService = null,
            IMessageEncoder messageEncoder = null,
            ILogger logger = null)
        {
            return new MessageService(
                networkService ?? Substitute.For<INetworkService>(),
                messageEncoder ?? Substitute.For<IMessageEncoder>(),
                logger ?? Substitute.For<ILogger>());
        }
    }
}
