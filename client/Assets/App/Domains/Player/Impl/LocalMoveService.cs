using System;
using App.Domains.Player.Api;
using R3;
using UnityEngine;

namespace App.Domains.Player.Impl
{
    public class LocalMoveService : IMoveService, IDisposable
    {
        private const float Speed = 2f;

        private readonly IPlayerView _player;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public LocalMoveService(IPlayerView player, IInputService inputService)
        {
            _player = player;

            inputService.Move
                .Subscribe(ApplyMove)
                .AddTo(_disposables);
        }

        public void Dispose() =>
            _disposables.Dispose();

        private void ApplyMove(Vector2 input) =>
            _player.SetPosition(_player.Position + input * Speed * Time.deltaTime);
    }
}