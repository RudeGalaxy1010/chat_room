using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App.Domains.Player.Impl
{
    public class PlayerFactory : IFactory<Vector2, Transform, IPlayerView>
    {
        private readonly PlayerView _playerPrefab;

        public PlayerFactory(PlayerView playerPrefab) =>
            _playerPrefab = playerPrefab;

        public IPlayerView Create(Vector2 position, Transform parent) =>
            Object.Instantiate(_playerPrefab, position, Quaternion.identity, parent);
    }
}