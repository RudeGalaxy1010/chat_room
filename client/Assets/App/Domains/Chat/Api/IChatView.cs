using App.Domains.UIManagement.Api;
using UnityEngine;

namespace App.Domains.Chat.Api
{
    public interface IChatView : IView
    {
        ITextInputView ChatInputView { get; }

        void AddMessage(string nickName, Color nickNameColor, string message);
    }
}