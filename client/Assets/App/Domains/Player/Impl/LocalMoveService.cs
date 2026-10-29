using System;
using System.Collections.Generic;
using App.Domains.Player.Api;
using R3;
using UnityEngine;

namespace App.Domains.Player.Impl
{
    public class LocalMoveService : ILocalMoveService, IDisposable
    {
        private readonly List<IPlayerModel> _players = new List<IPlayerModel>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public LocalMoveService(IInputService inputService)
        {
            inputService.Move
                .Subscribe(ApplyMove)
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
    }
}