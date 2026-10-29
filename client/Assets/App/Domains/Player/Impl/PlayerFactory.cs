using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App.Domains.Player.Impl
{
    public class PlayerFactory : IFactory<Vector2, IPlayerView>
    {
        private readonly PlayerView _playerPrefab;
        private readonly Transform _parent;

        public PlayerFactory(PlayerView playerPrefab, Transform parent)
        {
            _playerPrefab = playerPrefab;
            _parent = parent;
        }

        public IPlayerView Create(Vector2 position) =>
            Object.Instantiate(_playerPrefab, position, Quaternion.identity, _parent);
    }
}