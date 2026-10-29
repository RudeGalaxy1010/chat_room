using System;
using System.Collections.Generic;
using System.Linq;
using App.Domains.Player.Api;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace App.Domains.Player.Impl
{
    public class RemoteMoveService : IRemoteMoveService, IDisposable
    {
        private readonly Dictionary<int, IPlayerModel> _players = new Dictionary<int, IPlayerModel>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public RemoteMoveService()
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    List<int> keys = _players.Keys.ToList();

                    for (int i = 0; i < keys.Count; i++)
                    {
                        Vector2 shift = new Vector2(Random.value - 0.5f, Random.value - 0.5f);
                        IPlayerModel player = _players[keys[i]];
                        Vector2 position = player.Position + shift * player.Speed * Time.deltaTime;
                        ApplyMove(player.Id, position);
                    }
                })
                .AddTo(_disposables);
        }

        public void Dispose() =>
            _disposables.Dispose();

        private void ApplyMove(int id, Vector2 position) =>
            _players[id].SetPosition(position);

        public void Register(IPlayerModel player) =>
            _players[player.Id] = player;

        public void Unregister(IPlayerModel player) =>
            _players.Remove(player.Id);
    }
}