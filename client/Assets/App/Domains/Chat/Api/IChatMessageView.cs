using App.Domains.UIManagement.Api;

namespace App.Domains.Chat.Api
{
    public interface IChatMessageView : IView
    {
        void SetInfo(string nickName, string message);
        void Despawn();
    }
}