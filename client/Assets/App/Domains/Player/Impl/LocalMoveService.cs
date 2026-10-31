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
        private const float SyncPeriod = 1f;

        private readonly IMessageService _messageService;
        private readonly List<IPlayerModel> _players = new List<IPlayerModel>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public LocalMoveService(IMessageService messageService, IInputService inputService)
        {
            _messageService = messageService;

            inputService.Move
                .Subscribe(ApplyMove)
                .AddTo(_disposables);

            Observable
                .Interval(TimeSpan.FromSeconds(SyncPeriod))
                .Subscribe(_ => SyncPositions())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
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

        private void ApplyMove(Vector2 input)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                IPlayerModel player = _players[i];
                player.SetPosition(player.Position + input * player.Speed * Time.deltaTime);
            }
        }

        private void SyncPositions()
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