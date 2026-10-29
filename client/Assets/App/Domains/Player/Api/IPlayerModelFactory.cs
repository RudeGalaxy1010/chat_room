using App.Domains.Network.Impl;

namespace App.Domains.Player.Api
{
    public interface IPlayerModelFactory
    {
        IPlayerModel CreateLocalPlayer(PlayerEntity entity);
        IPlayerModel CreateRemotePlayer(PlayerEntity entity);
    }
}