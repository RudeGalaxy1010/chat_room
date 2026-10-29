using UnityEngine;

namespace App.Domains.Player.Api
{
    public interface IPlayerModel
    {
        int Id { get; }
        float Speed { get; }
        Vector2 Position { get; }
        void SetPosition(Vector2 position);
    }
}