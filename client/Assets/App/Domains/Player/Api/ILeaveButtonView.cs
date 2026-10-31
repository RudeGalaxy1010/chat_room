using System;

namespace App.Domains.Player.Api
{
    public interface ILeaveButtonView : UIManagement.Api.IView
    {
        event Action OnClick;
    }
}