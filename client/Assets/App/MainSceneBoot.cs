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
        private readonly ILocalContext _localContext;
        private readonly IPlayerModelFactory _playerFactory;
        private readonly Dictionary<int, IPlayerModel> _players = new Dictionary<int, IPlayerModel>();

        public MainSceneBoot(IMessageService messageService,
            ILocalContext localContext,
            IPlayerModelFactory playerFactory)
        {
            _messageService = messageService;
            _localContext = localContext;
            _playerFactory = playerFactory;

            _messageService.Register(MessageType.Join, OnJoin);
        }

        public void Initialize() =>
            _messageService.Send(new ReadyRequest());

        public void Dispose()
        {
            _messageService.Unregister(MessageType.Join, OnJoin);
            ClearPlayers();
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
    }
}
