using System;
using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App.Domains.Player.Impl
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IFactory<Vector2, IPlayerView> _playerViewFactory;
        private readonly IFactory<PlayerEntity, IPlayerView, Action<IPlayerModel>, IPlayerModel> _playerModelFactory;
        private readonly ILocalMoveService _localMoveService;
        private readonly IRemoteMoveService _remoteMoveService;

        public PlayerFactory(
            IFactory<Vector2, IPlayerView> playerViewFactory,
            IFactory<PlayerEntity, IPlayerView, Action<IPlayerModel>, IPlayerModel> playerModelFactory,
            ILocalMoveService localMoveService,
            IRemoteMoveService remoteMoveService)
        {
            _playerViewFactory = playerViewFactory;
            _playerModelFactory = playerModelFactory;
            _localMoveService = localMoveService;
            _remoteMoveService = remoteMoveService;
        }

        public IPlayerModel CreateLocalPlayer(PlayerEntity entity)
            => CreatePlayer(entity, _localMoveService);

        public IPlayerModel CreateRemotePlayer(PlayerEntity entity)
            => CreatePlayer(entity, _remoteMoveService);

        private IPlayerModel CreatePlayer(PlayerEntity entity, params IMoveService[] moveServices)
        {
            Vector2 position = new Vector2(entity.X, entity.Y);
            IPlayerView view = _playerViewFactory.Create(position);

            IPlayerModel player = _playerModelFactory.Create(entity, view, Unregister);

            for (int i = 0; i < moveServices.Length; i++)
            {
                moveServices[i].Register(player);
            }

            return player;

            void Unregister(IPlayerModel model)
            {
                for (int i = 0; i < moveServices.Length; i++)
                {
                    moveServices[i].Unregister(model);
                }
            }
        }
    }
}