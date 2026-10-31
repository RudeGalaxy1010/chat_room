using App.Domains.Network.Impl;
using App.Domains.Network.Impl.Messages;

namespace App.Domains.Player.Api
{
    public interface IPlayerModelFactory
    {
        IPlayerModel CreateLocalPlayer(PlayerEntity entity);
        IPlayerModel CreateRemotePlayer(PlayerEntity entity);
    }
}