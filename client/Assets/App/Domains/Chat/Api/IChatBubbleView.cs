using App.Domains.UIManagement.Api;

namespace App.Domains.Chat.Api
{
    public interface IChatBubbleView : IView
    {
        void SetInfo(string message);
    }
}