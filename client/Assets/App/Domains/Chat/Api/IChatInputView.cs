using System;

namespace App.Domains.Chat.Api
{
    public interface IChatInputView : IView
    {
        event Action<string> OnMessageSubmitted;

        void Clear();
    }
}