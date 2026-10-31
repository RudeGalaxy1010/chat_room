using App.Domains.UIManagement.Api;

namespace App.Domains.Chat.Api
{
    public interface IChatView : IView
    {
        ITextInputView ChatInputView { get; }

        void AddMessage(string nickName, string message);
    }
}