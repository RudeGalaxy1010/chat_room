using System;

namespace App.Domains.UIManagement.Api
{
    public interface ITextInputView : IView
    {
        event Action<string> OnSubmit;

        void SetPlaceholderText(string text);
        void SetLocked(bool isLocked);
        void Clear();
    }
}