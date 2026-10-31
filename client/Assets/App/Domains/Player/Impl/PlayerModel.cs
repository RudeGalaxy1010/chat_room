using System;
using System.Threading;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using App.Domains.Player.Api;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Domains.Player.Impl
{
    public class PlayerModel : IPlayerModel, IDisposable
    {
        private const float ChatBubbleShowTime = 4f;

        private readonly PlayerEntity _entity;
        private readonly IPlayerView _playerView;
        private readonly IMessageService _messageService;
        private readonly Action<IPlayerModel> _unregisterMovement;

        private CancellationTokenSource? _cancellationTokenSource;

        public int Id => _entity.Id;
        public float Speed => 2f;
        public Vector2 Position => _playerView.Position;

        public PlayerModel(PlayerEntity entity,
            IPlayerView playerView,
            IMessageService messageService,
            Action<IPlayerModel> unregisterMovement)
        {
            _entity = entity;
            _playerView = playerView;
            _messageService = messageService;
            _unregisterMovement = unregisterMovement;

            _playerView.SetName(_entity.Name);

            if (ColorUtility.TryParseHtmlString(_entity.Color, out Color color))
            {
                _playerView.SetColor(color);
            }
            else
            {
                Debug.LogError($"Failed to parse color '{_entity.Color}' for player '{_entity.Id}'");
            }

            _messageService.Register(MessageType.ChatMessage, OnChatMessageReceived);
        }

        public void Dispose()
        {
            _unregisterMovement(this);
            _cancellationTokenSource?.Cancel();
            _messageService.Unregister(MessageType.ChatMessage, OnChatMessageReceived);
            _playerView.Despawn();
        }

        public void SetPosition(Vector2 position) =>
            _playerView.SetPosition(position);

        private void OnChatMessageReceived(Response response)
        {
            ChatMessageResponse chatMessageResponse = (ChatMessageResponse)response;

            if (chatMessageResponse.IsOk && chatMessageResponse.Id == Id)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                ShowChatBubble(chatMessageResponse.Message, _cancellationTokenSource.Token).Forget();
            }
        }

        private async UniTask ShowChatBubble(string message, CancellationToken token)
        {
            _playerView.ChatBubbleView.SetInfo(message);
            await UniTask.Delay(TimeSpan.FromSeconds(ChatBubbleShowTime), cancellationToken: token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            _playerView.ChatBubbleView.SetInfo(string.Empty);
        }
    }
}
