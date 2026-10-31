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
    public class LocalMoveServiceTests
    {
        [Test]
        public void ReceivePosition_WhenCorrectionIsLarge_SetsPlayerPosition()
        {
            // Arrange
            IMessageService messageService = Substitute.For<IMessageService>();
            IInputService inputService = Substitute.For<IInputService>();
            inputService.Move.Returns(new Subject<Vector2>());
            IPlayerModel player = Substitute.For<IPlayerModel>();
            player.Id.Returns(7);
            player.Position.Returns(Vector2.zero);
            Action<Response> updateHandler = null;
            messageService.Register(Arg.Any<MessageType>(), Arg.Do<Action<Response>>(
                handler => updateHandler = handler));
            using LocalMoveService service = Create(messageService, inputService);
            service.Register(player);

            // Act
            updateHandler(new UpdatePositionResponse
            {
                Status = (int)MessageStatus.Ok,
                Id = 7,
                X = 2f,
                Y = 3f,
            });

            // Assert
            player.Received(1).SetPosition(new Vector2(2f, 3f));
        }

        [Test]
        public void ReceivePosition_WhenCorrectionIsSmall_DoesNotSetPlayerPosition()
        {
            // Arrange
            IMessageService messageService = Substitute.For<IMessageService>();
            IInputService inputService = Substitute.For<IInputService>();
            inputService.Move.Returns(new Subject<Vector2>());
            IPlayerModel player = Substitute.For<IPlayerModel>();
            player.Id.Returns(7);
            player.Position.Returns(Vector2.zero);
            Action<Response> updateHandler = null;
            messageService.Register(Arg.Any<MessageType>(), Arg.Do<Action<Response>>(
                handler => updateHandler = handler));
            using LocalMoveService service = Create(messageService, inputService);
            service.Register(player);

            // Act
            updateHandler(new UpdatePositionResponse
            {
                Status = (int)MessageStatus.Ok,
                Id = 7,
                X = 0.1f,
                Y = 0.1f,
            });

            // Assert
            player.DidNotReceive().SetPosition(Arg.Any<Vector2>());
        }

        private static LocalMoveService Create(
            IMessageService messageService = null,
            IInputService inputService = null)
        {
            return new LocalMoveService(
                messageService ?? Substitute.For<IMessageService>(),
                inputService ?? Substitute.For<IInputService>());
        }
    }
}
