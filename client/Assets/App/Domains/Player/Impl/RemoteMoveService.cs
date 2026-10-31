using System;
using System.Collections.Generic;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App.Domains.Player.Impl
{
    public class RemoteMoveService : IRemoteMoveService, ITickable, IDisposable
    {
        private const float InterpolationSmoothTime = 0.2f;

        private readonly IMessageService _messageService;
        private readonly Dictionary<int, RemotePlayerState> _players =
            new Dictionary<int, RemotePlayerState>();

        public RemoteMoveService(IMessageService messageService)
        {
            _messageService = messageService;
            _messageService.Register(MessageType.UpdatePosition, OnUpdatePosition);
        }

        public void Dispose()
        {
            _messageService.Unregister(MessageType.UpdatePosition, OnUpdatePosition);
            _players.Clear();
        }

        public void Tick() =>
            InterpolatePositions();

        private void OnUpdatePosition(Response response)
        {
            UpdatePositionResponse updatePositionResponse = (UpdatePositionResponse)response;

            if (!updatePositionResponse.IsOk ||
                !_players.TryGetValue(updatePositionResponse.Id, out RemotePlayerState state))
            {
                return;
            }

            state.TargetPosition = new Vector2(updatePositionResponse.X, updatePositionResponse.Y);
        }

        private void InterpolatePositions()
        {
            foreach (RemotePlayerState state in _players.Values)
            {
                Vector2 velocity = state.Velocity;
                Vector2 position = Vector2.SmoothDamp(
                    state.Player.Position,
                    state.TargetPosition,
                    ref velocity,
                    InterpolationSmoothTime);

                state.Velocity = velocity;
                state.Player.SetPosition(position);
            }
        }

        public void Register(IPlayerModel player)
        {
            _players[player.Id] = new RemotePlayerState(player);
        }

        public void Unregister(IPlayerModel player)
            => _players.Remove(player.Id);

        private sealed class RemotePlayerState
        {
            public RemotePlayerState(IPlayerModel player)
            {
                Player = player;
                TargetPosition = player.Position;
            }

            public IPlayerModel Player { get; }
            public Vector2 TargetPosition { get; set; }
            public Vector2 Velocity { get; set; }
        }
    }
}
