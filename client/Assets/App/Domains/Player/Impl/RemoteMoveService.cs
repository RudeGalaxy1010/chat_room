using System;
using System.Collections.Generic;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using UnityEngine;

namespace App.Domains.Player.Impl
{
    public class RemoteMoveService : IRemoteMoveService, IDisposable
    {
        private readonly IMessageService _messageService;
        private readonly Dictionary<int, IPlayerModel> _players = new Dictionary<int, IPlayerModel>();

        public RemoteMoveService(IMessageService messageService)
        {
            _messageService = messageService;
            _messageService.Register(MessageType.UpdatePosition, OnUpdatePosition);
        }

        public void Dispose() =>
            _messageService.Unregister(MessageType.UpdatePosition, OnUpdatePosition);

        private void OnUpdatePosition(Response response)
        {
            UpdatePositionResponse updatePositionResponse = (UpdatePositionResponse)response;

            if (updatePositionResponse.IsOk)
            {
                Vector2 position = new Vector2(updatePositionResponse.X, updatePositionResponse.Y);
                ApplyMove(updatePositionResponse.Id, position);
            }
        }

        private void ApplyMove(int id, Vector2 position) =>
            _players[id].SetPosition(position);

        public void Register(IPlayerModel player) =>
            _players[player.Id] = player;

        public void Unregister(IPlayerModel player) =>
            _players.Remove(player.Id);
    }
}