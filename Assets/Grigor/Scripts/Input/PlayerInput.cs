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

        private bool readMoveInput;
        private bool readLookInput;

        private Vector2 moveInputDirection;
        private Vector2 lookInputDirection;

        public Vector2 MoveInputDirection => moveInputDirection;
        public Vector2 LookInputDirection => lookInputDirection;

        public event Action MoveInputStartedEvent;
        public event Action MoveInputCanceledEvent;

        public event Action InteractInputStartedEvent;
        public event Action InteractInputCanceledEvent;

        public event Action LookInputStartedEvent;
        public event Action LookInputCanceledEvent;

        public event Action JumpInputStartedEvent;
        public event Action JumpInputCanceledEvent;


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

            playerInputActions.Player.Look.started += OnLookInputStarted;
            playerInputActions.Player.Look.canceled += OnLookInputCanceled;

            playerInputActions.Player.Jump.started += OnJumpInputStarted;
            playerInputActions.Player.Jump.canceled += OnJumpInputCanceled;
        }

        private void Update()
        {
            if (readMoveInput)
            {
                moveInputDirection = playerInputActions.Player.Move.ReadValue<Vector2>();
            }

            if (readLookInput)
            {
                lookInputDirection = playerInputActions.Player.Look.ReadValue<Vector2>();
            }
        }

        private void OnMoveInputStarted(InputAction.CallbackContext context)
        {
            readMoveInput = true;

            MoveInputStartedEvent?.Invoke();
        }

        private void OnMoveInputCanceled(InputAction.CallbackContext context)
        {
            moveInputDirection = Vector2.zero;

            readMoveInput = false;

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

        private void OnLookInputStarted(InputAction.CallbackContext context)
        {
            readLookInput = true;

            LookInputStartedEvent?.Invoke();
        }

        private void OnLookInputCanceled(InputAction.CallbackContext context)
        {
            readLookInput = false;

            LookInputCanceledEvent?.Invoke();
        }

        private void OnJumpInputStarted(InputAction.CallbackContext context)
        {
            JumpInputStartedEvent?.Invoke();
        }

        private void OnJumpInputCanceled(InputAction.CallbackContext context)
        {
            JumpInputCanceledEvent?.Invoke();
        }
    }
}
