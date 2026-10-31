using App.Domains.Chat.Api;
using UnityEngine;

namespace App.Domains.Player.Api
{
    public interface IPlayerView
    {
        Vector2 Position { get; }
        IChatBubbleView ChatBubbleView { get; }

        void SetPosition(Vector2 position);
        void SetName(string value);
        void SetColor(Color color);
        void Despawn();
    }
}