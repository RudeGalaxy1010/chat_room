using System;
using App.Domains.Network.Impl;
using App.Domains.Player.Api;
using UnityEngine;

namespace App.Domains.Player.Impl
{
    public class PlayerModel : IPlayerModel, IDisposable
    {
        private readonly PlayerEntity _entity;
        private readonly IPlayerView _playerView;
        private readonly IMoveService _moveService;

        public int Id => _entity.Id;
        public float Speed => 2f;
        public Vector2 Position => _playerView.Position;

        public PlayerModel(PlayerEntity entity, IPlayerView playerView, IMoveService moveService)
        {
            _entity = entity;
            _playerView = playerView;
            _moveService = moveService;

            _playerView.SetName(_entity.Name);

            if (ColorUtility.TryParseHtmlString(_entity.Color, out Color color))
            {
                _playerView.SetColor(color);
            }
            else
            {
                Debug.LogError($"Failed to parse color '{_entity.Color}' for player '{_entity.Id}'");
            }

            _moveService.Register(this);
        }

        public void Dispose()
        {
            _moveService.Unregister(this);
            _playerView.Despawn();
        }

        public void SetPosition(Vector2 position) =>
            _playerView.SetPosition(position);
    }
}