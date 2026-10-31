using App.Domains.UIManagement.Impl;
using UnityEngine;
using Zenject;

namespace App.Installers
{
    public class InitSceneInstaller : MonoInstaller
    {
        [SerializeField] private TextInputView _textInputView = null!;

        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<InitSceneBoot>()
                .AsSingle()
                .WithArguments(_textInputView);
        }
    }
}