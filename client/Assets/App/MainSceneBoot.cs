using System;
using System.Collections.Generic;
using System.Linq;
using App.Domains.Core.Api;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using R3;
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
        private readonly ILogger _logger;
        private readonly Dictionary<int, IPlayerModel> _players = new Dictionary<int, IPlayerModel>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isLeaving;

        public MainSceneBoot(IMessageService messageService,
            INetworkService networkService,
            ILocalContext localContext,
            IPlayerFactory playerFactory,
            ISceneLoadService sceneLoadService,
            ILeaveButtonView leaveButton,
            ILogger logger)
        {
            _messageService = messageService;
            _networkService = networkService;
            _localContext = localContext;
            _playerFactory = playerFactory;
            _sceneLoadService = sceneLoadService;
            _leaveButton = leaveButton;
            _logger = logger;

            _messageService.Register(MessageType.Join, OnJoin);
            _messageService.Register(MessageType.Leave, OnLeave);
            _leaveButton.OnClick += OnLeaveButtonClick;

            _networkService.IsConnected
                .Subscribe(OnConnectionChanged)
                .AddTo(_disposables);
        }

        public void Initialize() =>
            _messageService.Send(new ReadyRequest());

        public void Dispose()
        {
            _isLeaving = true;
            _disposables.Dispose();
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
                _logger.LogError("Join response failed");
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
            _isLeaving = true;
            _sceneLoadService.LoadScene(SceneName.Init);
        }

        private void OnConnectionChanged(bool isConnected)
        {
            if (isConnected)
            {
                return;
            }

            if (_isLeaving)
            {
                return;
            }

            ClearPlayers();
            _sceneLoadService.LoadScene(SceneName.Init);
        }
    }
}
