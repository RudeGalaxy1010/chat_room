using App.Domains.Core.Impl;
using App.Domains.Network.Impl;
using Zenject;

namespace App.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ConsoleLogger>().AsSingle();
            Container.BindInterfacesTo<LocalContext>().AsSingle();
            Container.BindInterfacesTo<SceneLoadService>().AsSingle();

            InstallNetwork(Container);
        }

        private static void InstallNetwork(DiContainer container)
        {
            container.BindInterfacesTo<NetworkService>().AsSingle();
            container.BindInterfacesTo<JsonMessageEncoder>().AsSingle();
            container.BindInterfacesTo<MessageService>().AsSingle();
        }
    }
}