namespace App.Domains.Player.Api
{
    public interface IMoveService
    {
        void Register(IPlayerModel player);
        void Unregister(IPlayerModel player);
    }
}