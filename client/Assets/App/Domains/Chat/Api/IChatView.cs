namespace App.Domains.Chat.Api
{
    public interface IChatView : IView
    {
        IChatInputView ChatInputView { get; }

        void AddMessage(int id, string nickName, string message);
        void RemoveMessage(int id);
    }
}