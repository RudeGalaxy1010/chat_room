using App.Domains.Chat.Impl;
using App.Domains.Network.Impl.Messages;
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
        [SerializeField] private ChatView _chatView = null!;
        [SerializeField] private LeaveButtonView _leaveButton = null!;

        public override void InstallBindings()
        {
            InstallPlayer(Container);
            InstallChat(Container);

            Container
                .BindInterfacesTo<MainSceneBoot>()
                .AsSingle()
                .WithArguments(_leaveButton);
        }

        private void InstallPlayer(DiContainer container)
        {
            container
                .BindInterfacesTo<PlayerViewFactory>()
                .AsSingle()
                .WithArguments(_playerPrefab, _root);

            container
                .BindIFactory<PlayerEntity, IPlayerView, IPlayerModel>()
                .To<PlayerModel>()
                .AsSingle();

            container.BindInterfacesTo<InputService>().AsSingle();
            container.BindInterfacesTo<LocalMoveService>().AsSingle();
            container.BindInterfacesTo<RemoteMoveService>().AsSingle();
            container.BindInterfacesTo<PlayerFactory>().AsSingle();
        }

        private void InstallChat(DiContainer container)
        {
            container.BindInterfacesTo<ChatService>().AsSingle().WithArguments(_chatView);
        }
    }
}