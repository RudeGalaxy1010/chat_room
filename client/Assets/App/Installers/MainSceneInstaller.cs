using App.Domains.Chat.Impl;
using App.Domains.Player.Impl;
using UnityEngine;
using Zenject;

namespace App.Installers
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private Transform _root = null!;
        [SerializeField] private PlayerView _playerPrefab = null!;
        [SerializeField] private ChatView _chatView = null!;

        public override void InstallBindings()
        {
            InstallPlayer(Container);
            InstallChat(Container);

            Container.BindInterfacesTo<MainSceneBoot>().AsSingle();
        }

        private void InstallPlayer(DiContainer container)
        {
            container.BindInterfacesTo<PlayerFactory>().AsSingle().WithArguments(_playerPrefab, _root);
            container.BindInterfacesTo<InputService>().AsSingle();
            container.BindInterfacesTo<LocalMoveService>().AsSingle();
            container.BindInterfacesTo<RemoteMoveService>().AsSingle();
            container.BindInterfacesTo<PlayerModelFactory>().AsSingle();
        }

        private void InstallChat(DiContainer container)
        {
            container.BindInterfacesTo<ChatService>().AsSingle().WithArguments(_chatView);
        }
    }
}