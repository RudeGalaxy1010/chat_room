using System;
using App.Domains.Network.Impl;
using App.Domains.Player.Api;
using Zenject;

namespace App
{
    public class Boot : IInitializable, IDisposable
    {
        private readonly IPlayerModelFactory _playerFactory;

        private IPlayerModel? _localPlayer;

        public Boot(IPlayerModelFactory playerFactory) =>
            _playerFactory = playerFactory;

        public void Initialize() =>
            _localPlayer = _playerFactory.CreateLocalPlayer(new PlayerEntity
            (
                -1,
                1,
                2,
                "Test",
                "#FFFFF2"
            ));

        public void Dispose()
        {
            if (_localPlayer is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}