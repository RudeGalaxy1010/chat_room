using App.Domains.Chat.Api;
using TMPro;
using UnityEngine;

namespace App.Domains.Chat.Impl
{
    public class ChatMessageView : View, IChatMessageView
    {
        [SerializeField] private TMP_Text _nameLabel = null!;
        [SerializeField] private TMP_Text _textLabel = null!;

        public void SetInfo(string nickName, string message)
        {
            _nameLabel.text = nickName;
            _textLabel.text = message;
        }

        public void Despawn()
        {
            if (this == null)
            {
                return;
            }

            Destroy(gameObject);
        }
    }
}