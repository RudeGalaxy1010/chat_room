using App.Domains.Network.Impl.Messages;

namespace App.Domains.Player.Api
{
    public interface IPlayerFactory
    {
        IPlayerModel CreateLocalPlayer(PlayerEntity entity);
        IPlayerModel CreateRemotePlayer(PlayerEntity entity);
    }
}