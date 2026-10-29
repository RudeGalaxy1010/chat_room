using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App
{
    public class Boot : IInitializable
    {
        private readonly IFactory<Vector2, Transform, IPlayerView> _playerFactory;
        private readonly Transform _root;

        public Boot(IFactory<Vector2, Transform, IPlayerView> playerFactory, Transform root)
        {
            _playerFactory = playerFactory;
            _root = root;
        }

        public void Initialize()
        {
            IPlayerView player = _playerFactory.Create(Vector2.zero, _root);
        }
    }
}