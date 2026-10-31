using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App.Domains.Player.Impl
{
    public class PlayerModelFactory : IPlayerModelFactory
    {
        private readonly IFactory<Vector2, IPlayerView> _playerViewFactory;
        private readonly IFactory<PlayerEntity, IPlayerView, IPlayerModel> _playerModelFactory;
        private readonly ILocalMoveService _localMoveService;
        private readonly IRemoteMoveService _remoteMoveService;

        public PlayerModelFactory(
            IFactory<Vector2, IPlayerView> playerViewFactory,
            IFactory<PlayerEntity, IPlayerView, IPlayerModel> playerModelFactory,
            ILocalMoveService localMoveService,
            IRemoteMoveService remoteMoveService)
        {
            _playerViewFactory = playerViewFactory;
            _playerModelFactory = playerModelFactory;
            _localMoveService = localMoveService;
            _remoteMoveService = remoteMoveService;
        }

        public IPlayerModel CreateLocalPlayer(PlayerEntity entity)
        {
            IPlayerModel player = CreatePlayer(entity);
            _localMoveService.Register(player);
            _remoteMoveService.Register(player);
            return player;
        }

        public IPlayerModel CreateRemotePlayer(PlayerEntity entity)
        {
            IPlayerModel player = CreatePlayer(entity);
            _remoteMoveService.Register(player);
            return player;
        }

        private IPlayerModel CreatePlayer(PlayerEntity entity)
        {
            Vector2 position = new Vector2(entity.X, entity.Y);
            IPlayerView view = _playerViewFactory.Create(position);
            return _playerModelFactory.Create(entity, view);
        }
    }
}