using System;
using App.Domains.Player.Api;
using UnityEngine;
using UnityEngine.UI;

namespace App.Domains.Player.Impl
{
    public class LeaveButtonView : UIManagement.Impl.View, ILeaveButtonView
    {
        [SerializeField] private Button _button = null!;

        public event Action OnClick = delegate { };

        private void OnEnable() =>
            _button.onClick.AddListener(OnButtonClick);

        private void OnDisable() =>
            _button.onClick.RemoveListener(OnButtonClick);

        private void OnButtonClick() =>
            OnClick.Invoke();
    }
}