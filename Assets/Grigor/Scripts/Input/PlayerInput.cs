using System;
using CardboardCore.DI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grigor.Input
{
    [Injectable]
    public class PlayerInput : MonoBehaviour
    {
        private PlayerInputActions playerInputActions;

        public event Action<Vector2> MoveInputStartedEvent;
        public event Action MoveInputCanceledEvent;
        public event Action InteractInputStartedEvent;
        public event Action InteractInputCanceledEvent;

        private void Awake()
        {
            playerInputActions = new PlayerInputActions();

            playerInputActions.Enable();
        }

        private void OnEnable()
        {
            playerInputActions.Player.Move.started += OnMoveInputStarted;
            playerInputActions.Player.Move.canceled += OnMoveInputCanceled;

            playerInputActions.Player.Interact.started += OnInteractInputStarted;
            playerInputActions.Player.Interact.canceled += OnInteractInputCanceled;
        }

        private void OnMoveInputStarted(InputAction.CallbackContext context)
        {
            MoveInputStartedEvent?.Invoke(context.ReadValue<Vector2>());
        }

        private void OnMoveInputCanceled(InputAction.CallbackContext context)
        {
            MoveInputCanceledEvent?.Invoke();
        }

        private void OnInteractInputStarted(InputAction.CallbackContext context)
        {
            InteractInputCanceledEvent?.Invoke();
        }

        private void OnInteractInputCanceled(InputAction.CallbackContext context)
        {
            InteractInputStartedEvent?.Invoke();
        }

    }
}
