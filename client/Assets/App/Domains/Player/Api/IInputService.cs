using R3;
using UnityEngine;

namespace App.Domains.Player.Api
{
    public interface IInputService
    {
        Observable<Vector2> Move { get; }
    }
}