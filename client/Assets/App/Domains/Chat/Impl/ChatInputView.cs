using System;
using App.Domains.Chat.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Domains.Chat.Impl
{
    public class ChatInputView : View, IChatInputView
    {
        [SerializeField] private TMP_InputField _inputField = null!;
        [SerializeField] private Button _button = null!;

        public event Action<string> OnMessageSubmitted = delegate { };

        private void OnEnable() =>
            _button.onClick.AddListener(OnButtonClick);

        private void OnDisable() =>
            _button.onClick.RemoveListener(OnButtonClick);

        public void Clear() =>
            _inputField.text = string.Empty;

        private void OnButtonClick() =>
            OnMessageSubmitted.Invoke(_inputField.text);
    }
}