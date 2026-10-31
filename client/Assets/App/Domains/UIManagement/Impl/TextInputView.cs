using System;
using App.Domains.UIManagement.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Domains.UIManagement.Impl
{
    public class TextInputView : View, ITextInputView
    {
        [SerializeField] private TMP_Text _placeholderText = null!;
        [SerializeField] private TMP_InputField _inputField = null!;
        [SerializeField] private Button _submitButton = null!;

        public event Action<string> OnSubmit = delegate { };

        private void OnEnable() =>
            _submitButton.onClick.AddListener(OnClick);

        private void OnDisable() =>
            _submitButton.onClick.RemoveListener(OnClick);

        public void SetPlaceholderText(string text) =>
            _placeholderText.text = text;

        public void SetLocked(bool isLocked)
        {
            _inputField.interactable = !isLocked;
            _submitButton.interactable = !isLocked;
        }

        public void Clear() =>
            _inputField.text = string.Empty;

        private void OnClick() =>
            OnSubmit.Invoke(_inputField.text);
    }
}