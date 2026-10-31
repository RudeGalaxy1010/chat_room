using App.Domains.Chat.Api;
using App.Domains.UIManagement.Impl;
using TMPro;
using UnityEngine;

namespace App.Domains.Chat.Impl
{
    public class ChatMessageView : View, IChatMessageView
    {
        [SerializeField] private TMP_Text _nameLabel = null!;
        [SerializeField] private TMP_Text _textLabel = null!;

        public void SetInfo(string nickName, Color nickNameColor, string message)
        {
            _nameLabel.color = nickNameColor;
            _nameLabel.text = $"{nickName}: ";
            _textLabel.text = message;
        }
    }
}