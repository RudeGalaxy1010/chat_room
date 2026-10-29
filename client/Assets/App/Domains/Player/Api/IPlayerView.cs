using UnityEngine;

namespace App.Domains.Player.Api
{
    public interface IPlayerView
    {
        Vector3 Position { get; }
        void SetPosition(Vector2 position);
        void SetName(string value);
        void SetColor(Color color);
    }
}