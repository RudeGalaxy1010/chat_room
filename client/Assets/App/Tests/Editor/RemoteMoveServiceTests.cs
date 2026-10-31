using System;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using App.Domains.Player.Impl;
using NSubstitute;
using NUnit.Framework;
using R3;
using UnityEngine;

namespace App.Tests.Domains.Player
{
    public class RemoteMoveServiceTests
    {
        [Test]
        public void ReceivePosition_ForRegisteredPlayer_UpdatesPlayerOnTick()
        {
            // Arrange
            IMessageService messageService = Substitute.For<IMessageService>();
            Action<Response> updateHandler = null;
            messageService.Register(Arg.Any<MessageType>(), Arg.Do<Action<Response>>(
                handler => updateHandler = handler));
            IPlayerModel player = Substitute.For<IPlayerModel>();
            player.Id.Returns(7);
            player.Position.Returns(Vector2.zero);
            using RemoteMoveService service = Create(messageService);
            service.Register(player);

            updateHandler(new UpdatePositionResponse
            {
                Status = (int)MessageStatus.Ok,
                Id = 7,
                X = 2f,
                Y = 3f,
            });

            // Act
            service.Tick();

            // Assert
            player.Received(1).SetPosition(Arg.Any<Vector2>());
        }

        [Test]
        public void Tick_AfterUnregister_DoesNotUpdatePlayer()
        {
            // Arrange
            IMessageService messageService = Substitute.For<IMessageService>();
            IPlayerModel player = Substitute.For<IPlayerModel>();
            player.Id.Returns(7);
            player.Position.Returns(Vector2.zero);
            using RemoteMoveService service = Create(messageService);
            service.Register(player);
            service.Unregister(player);

            // Act
            service.Tick();

            // Assert
            player.DidNotReceive().SetPosition(Arg.Any<Vector2>());
        }

        private static RemoteMoveService Create(IMessageService messageService = null)
        {
            return new RemoteMoveService(messageService ?? Substitute.For<IMessageService>());
        }
    }
}
