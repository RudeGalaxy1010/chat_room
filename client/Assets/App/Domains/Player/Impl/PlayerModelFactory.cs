using App.Domains.Network.Impl;
using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App.Domains.Player.Impl
{
    public class PlayerModelFactory : IPlayerModelFactory
    {
        private readonly IFactory<Vector2, IPlayerView> _playerFactory;
        private readonly ILocalMoveService _localMoveService;
        private readonly IRemoteMoveService _remoteMoveService;

        public PlayerModelFactory(IFactory<Vector2, IPlayerView> playerFactory,
            ILocalMoveService localMoveService,
            IRemoteMoveService remoteMoveService)
        {
            _playerFactory = playerFactory;
            _localMoveService = localMoveService;
            _remoteMoveService = remoteMoveService;
        }

        public IPlayerModel CreateLocalPlayer(PlayerEntity entity) =>
            CreatePlayer(entity, _localMoveService);

        public IPlayerModel CreateRemotePlayer(PlayerEntity entity) =>
            CreatePlayer(entity, _remoteMoveService);

        private IPlayerModel CreatePlayer(PlayerEntity entity, IMoveService moveService)
        {
            Vector2 position = new Vector2(entity.X, entity.Y);
            IPlayerView view = _playerFactory.Create(position);
            return new PlayerModel(entity, view, moveService);
        }
    }
}