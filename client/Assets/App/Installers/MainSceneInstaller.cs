using App.Domains.Player.Api;
using App.Domains.Player.Impl;
using UnityEngine;
using Zenject;

namespace App.Installers
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private Transform _root = null!;
        [SerializeField] private PlayerView _playerPrefab = null!;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<Boot>().AsSingle();
            Container.BindInterfacesTo<PlayerFactory>().AsSingle().WithArguments(_playerPrefab, _root);
            Container.BindInterfacesTo<InputService>().AsSingle();
            Container.BindIFactory<IPlayerView, IMoveService>().To<LocalMoveService>().AsSingle();
            Container.BindInterfacesTo<PlayerModelFactory>().AsSingle();
        }
    }
}