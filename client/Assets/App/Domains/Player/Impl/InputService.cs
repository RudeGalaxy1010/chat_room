using System;
using App.Domains.Player.Api;
using App.Input;
using R3;
using UnityEngine;
using Zenject;

namespace App.Domains.Player.Impl
{
    public class InputService : IInputService, ITickable, IDisposable
    {
        private readonly PlayerInputActions _inputActions;
        private readonly Subject<Vector2> _move;

        public Observable<Vector2> Move => _move;

        public InputService()
        {
            _inputActions = new PlayerInputActions();
            _move = new Subject<Vector2>();
            _inputActions.Enable();
        }

        public void Dispose()
        {
            _inputActions.Disable();
            _inputActions.Dispose();
            _move.Dispose();
        }

        public void Tick() =>
            _move.OnNext(_inputActions.Player.Move.ReadValue<Vector2>());
    }
}