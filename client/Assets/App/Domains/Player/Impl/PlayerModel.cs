using System;
using App.Domains.Network.Impl;
using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App.Domains.Player.Impl
{
    public class PlayerModel : IPlayerModel, IDisposable
    {
        private readonly PlayerEntity _entity;
        private readonly IPlayerView _playerView;
        private readonly IMoveService _moveService;
        private readonly bool _isLocal;

        public int Id => _entity.Id;

        public PlayerModel(PlayerEntity entity,
            IFactory<Vector2, IPlayerView> playerFactory,
            IFactory<IPlayerView, IMoveService> moveServiceFactory)
        {
            _entity = entity;
            _playerView = playerFactory.Create(new Vector2(_entity.X, _entity.Y));
            _moveService = moveServiceFactory.Create(_playerView);

            _playerView.SetName(_entity.Name);

            if (ColorUtility.TryParseHtmlString(_entity.Color, out Color color))
            {
                _playerView.SetColor(color);
            }
            else
            {
                Debug.LogError($"Failed to parse color '{_entity.Color}' for player '{_entity.Id}'");
            }
        }

        public void Dispose()
        {
            if (_moveService is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _playerView.Despawn();
        }
    }
}