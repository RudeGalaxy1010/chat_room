using App.Domains.UIManagement.Api;
using UnityEngine;

namespace App.Domains.Chat.Api
{
    public interface IChatMessageView : IView
    {
        void SetInfo(string nickName, Color nickNameColor, string message);
    }
}