using System;
using App.Domains.Core.Api;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using App.Domains.UIManagement.Api;
using R3;
using UnityEngine;
using Zenject;

namespace App
{
    public class InitSceneBoot : IInitializable, IDisposable
    {
        private readonly INetworkService _networkService;
        private readonly IMessageService _messageService;
        private readonly ISceneLoadService _sceneLoadService;
        private readonly ITextInputView _textInputView;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isConnectRequestPending;

        public InitSceneBoot(INetworkService networkService,
            IMessageService messageService,
            ISceneLoadService sceneLoadService,
            ITextInputView textInputView)
        {
            _networkService = networkService;
            _messageService = messageService;
            _sceneLoadService = sceneLoadService;
            _textInputView = textInputView;

            _networkService.IsConnected
                .Subscribe(UpdateConnection)
                .AddTo(_disposables);
        }

        public void Initialize()
        {
            _textInputView.OnSubmit += OnConnectSubmit;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _textInputView.OnSubmit -= OnConnectSubmit;
            _messageService.Unregister(MessageType.Connect, OnConnected);
        }

        private void UpdateConnection(bool isConnected)
        {
            if (isConnected)
            {
                _textInputView.Clear();
                _textInputView.SetPlaceholderText("Enter nickName...");
            }
            else
            {
                _textInputView.SetPlaceholderText("Enter ip:port...");
                _isConnectRequestPending = false;
                _messageService.Unregister(MessageType.Connect, OnConnected);
            }
        }

        private void OnConnectSubmit(string text)
        {
            if (_networkService.IsConnected.CurrentValue)
            {
                if (_isConnectRequestPending)
                {
                    return;
                }

                _isConnectRequestPending = true;
                _messageService.Register(MessageType.Connect, OnConnected);
                _messageService.Send(new ConnectRequest
                {
                    NickName = text,
                });
            }
            else
            {
                string[] address = text.Trim().Split(':');

                if (address.Length != 2 ||
                    string.IsNullOrWhiteSpace(address[0]) ||
                    !int.TryParse(address[1].Trim(), out int port) ||
                    port is < 1 or > 65535)
                {
                    Debug.LogError("Invalid server address. Expected host:port");
                    return;
                }

                _networkService.Connect(address[0], port);
            }
        }

        private void OnConnected(Response response)
        {
            _isConnectRequestPending = false;
            _messageService.Unregister(MessageType.Connect, OnConnected);
            ConnectResponse connectResponse = (ConnectResponse)response;

            if (!connectResponse.IsOk)
            {
                Debug.LogError("Connection failed");
                return;
            }

            _sceneLoadService.LoadScene(SceneName.Main);
        }
    }
}
