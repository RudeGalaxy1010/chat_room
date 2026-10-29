using App.Domains.Network.Impl;
using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App.Domains.Player.Impl
{
    public class PlayerModelFactory : IPlayerModelFactory
    {
        private readonly IFactory<Vector2, IPlayerView> _playerFactory;
        private readonly IFactory<IPlayerView, IMoveService> _localMoveServiceFactory;

        public PlayerModelFactory(IFactory<Vector2, IPlayerView> playerFactory,
            IFactory<IPlayerView, IMoveService> localMoveServiceFactory)
        {
            _playerFactory = playerFactory;
            _localMoveServiceFactory = localMoveServiceFactory;
        }

        public IPlayerModel CreateLocalPlayer(PlayerEntity entity) =>
            new PlayerModel(entity, _playerFactory, _localMoveServiceFactory);
    }
}