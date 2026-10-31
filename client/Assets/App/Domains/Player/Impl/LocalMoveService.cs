using System;
using System.Collections.Generic;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using R3;
using UnityEngine;

namespace App.Domains.Player.Impl
{
    public class LocalMoveService : ILocalMoveService, IDisposable
    {
        private const float ServerCorrectionDistance = 0.5f;
        private const float PositionSyncInterval = 0.2f;

        private readonly IMessageService _messageService;
        private readonly List<IPlayerModel> _players = new List<IPlayerModel>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public LocalMoveService(IMessageService messageService, IInputService inputService)
        {
            _messageService = messageService;

            inputService.Move
                .Subscribe(ApplyInput)
                .AddTo(_disposables);

            Observable
                .Interval(TimeSpan.FromSeconds(PositionSyncInterval))
                .Subscribe(_ => SyncPosition())
                .AddTo(_disposables);

            _messageService.Register(MessageType.UpdatePosition, OnUpdatePosition);
        }

        public void Dispose()
        {
            _messageService.Unregister(MessageType.UpdatePosition, OnUpdatePosition);
            _disposables.Dispose();
            _players.Clear();
        }

        public void Register(IPlayerModel player)
        {
            if (!_players.Contains(player))
            {
                _players.Add(player);
            }
        }

        public void Unregister(IPlayerModel player) =>
            _players.Remove(player);

        private void OnUpdatePosition(Response response)
        {
            UpdatePositionResponse updatePositionResponse = (UpdatePositionResponse)response;

            if (!updatePositionResponse.IsOk)
            {
                return;
            }

            Vector2 serverPosition = new Vector2(updatePositionResponse.X, updatePositionResponse.Y);

            for (int i = 0; i < _players.Count; i++)
            {
                IPlayerModel player = _players[i];

                if (player.Id == updatePositionResponse.Id &&
                    Vector2.Distance(player.Position, serverPosition) > ServerCorrectionDistance)
                {
                    player.SetPosition(serverPosition);
                    return;
                }
            }
        }

        private void ApplyInput(Vector2 input)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                IPlayerModel player = _players[i];
                player.SetPosition(player.Position + input * player.Speed * Time.deltaTime);
            }
        }

        private void SyncPosition()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                IPlayerModel player = _players[i];

                _messageService.Send(new UpdatePositionRequest
                {
                    Id = player.Id,
                    X = player.Position.x,
                    Y = player.Position.y,
                });
            }
        }
    }
}
