using App.Domains.Chat.Api;
using App.Domains.UIManagement.Impl;
using TMPro;
using UnityEngine;

namespace App.Domains.Chat.Impl
{
    public class ChatBubbleView : View, IChatBubbleView
    {
        [SerializeField] private TMP_Text _textLabel = null!;

        public void SetInfo(string message)
        {
            gameObject.SetActive(!string.IsNullOrEmpty(message));
            _textLabel.text = message;
        }
    }
}