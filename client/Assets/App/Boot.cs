using System;
using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App
{
    public class Boot : IInitializable, IDisposable
    {
        private readonly IFactory<Vector2, IPlayerView> _playerFactory;
        private readonly IFactory<IPlayerView, IMoveService> _moveServiceFactory;

        private IMoveService? _moveService;

        public Boot(IFactory<Vector2, IPlayerView> playerFactory,
            IFactory<IPlayerView, IMoveService> moveServiceFactory)
        {
            _playerFactory = playerFactory;
            _moveServiceFactory = moveServiceFactory;
        }

        public void Initialize()
        {
            IPlayerView player = _playerFactory.Create(Vector2.zero);
            _moveService = _moveServiceFactory.Create(player);
        }

        public void Dispose()
        {
            if (_moveService is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}