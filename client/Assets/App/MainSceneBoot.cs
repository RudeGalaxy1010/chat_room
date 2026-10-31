using System;
using System.Collections.Generic;
using System.Linq;
using App.Domains.Core.Api;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using UnityEngine;
using Zenject;

namespace App
{
    public class MainSceneBoot : IInitializable, IDisposable
    {
        private readonly IMessageService _messageService;
        private readonly INetworkService _networkService;
        private readonly ILocalContext _localContext;
        private readonly IPlayerFactory _playerFactory;
        private readonly ISceneLoadService _sceneLoadService;
        private readonly ILeaveButtonView _leaveButton;
        private readonly Dictionary<int, IPlayerModel> _players = new Dictionary<int, IPlayerModel>();

        public MainSceneBoot(IMessageService messageService,
            INetworkService networkService,
            ILocalContext localContext,
            IPlayerFactory playerFactory,
            ISceneLoadService sceneLoadService,
            ILeaveButtonView leaveButton)
        {
            _messageService = messageService;
            _networkService = networkService;
            _localContext = localContext;
            _playerFactory = playerFactory;
            _sceneLoadService = sceneLoadService;
            _leaveButton = leaveButton;

            _messageService.Register(MessageType.Join, OnJoin);
            _messageService.Register(MessageType.Leave, OnLeave);
            _leaveButton.OnClick += OnLeaveButtonClick;
        }

        public void Initialize() =>
            _messageService.Send(new ReadyRequest());

        public void Dispose()
        {
            _leaveButton.OnClick -= OnLeaveButtonClick;
            _messageService.Unregister(MessageType.Join, OnJoin);
            _messageService.Unregister(MessageType.Leave, OnLeave);
            ClearPlayers();
            _networkService.Disconnect();
        }

        private void OnJoin(Response response)
        {
            JoinResponse joinResponse = (JoinResponse)response;

            if (!joinResponse.IsOk)
            {
                Debug.LogError("Join response failed");
                return;
            }

            IPlayerModel remotePlayer = joinResponse.Player.Id == _localContext.Id
                ? _playerFactory.CreateLocalPlayer(joinResponse.Player)
                : _playerFactory.CreateRemotePlayer(joinResponse.Player);

            _players.Add(remotePlayer.Id, remotePlayer);
        }

        private void OnLeave(Response response)
        {
            LeaveResponse leaveResponse = (LeaveResponse)response;

            if (!leaveResponse.IsOk || !_players.Remove(leaveResponse.Id, out IPlayerModel player))
            {
                return;
            }

            if (player is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void ClearPlayers()
        {
            List<IPlayerModel> playersList = _players.Values.ToList();

            for (int i = 0; i < playersList.Count; i++)
            {
                if (playersList[i] is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _players.Clear();
        }

        private void OnLeaveButtonClick()
        {
            // Socket will be closed at dispose automatically and leave will be completed
            _sceneLoadService.LoadScene(SceneName.Init);
        }
    }
}