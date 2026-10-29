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
            Container.BindInterfacesTo<Boot>().AsSingle().WithArguments(_root);
            Container.BindInterfacesTo<PlayerFactory>().AsSingle().WithArguments(_playerPrefab);
        }
    }
}