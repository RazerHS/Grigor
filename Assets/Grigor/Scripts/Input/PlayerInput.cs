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

        public event Action SkipInputStartedEvent;
        public event Action SkipInputCanceledEvent;

        public event Action PhoneInputStartedEvent;

        public event Action EndDayInputStartedEvent;

        public event Action CatnipInputStartedEvent;
        public event Action CatnipInputCanceledEvent;

        public event Action OnRefreshInputStartedEvent;

        public event Action<float> ScrollInputStartedEvent;

        public event Action PauseInputStartedEvent;

        public event Action TimeskipInputStartedEvent;

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

            playerInputActions.Player.Skip.started += OnSkipInputStarted;
            playerInputActions.Player.Skip.canceled += OnSkipInputCanceled;

            playerInputActions.Player.Phone.started += OnPhoneInputStarted;

            playerInputActions.Player.EndDay.started += OnEndDayInputStarted;

            playerInputActions.Player.Catnip.started += OnCatnipInputStarted;

            playerInputActions.Player.Scroll.started += OnScrollInputStarted;

            playerInputActions.Player.Pause.started += OnPauseInputStarted;

            playerInputActions.Player.Refresh.started += OnRefreshInputStarted;

            playerInputActions.Player.Timeskip.started += OnTimeskipInputStarted;
        }

        private void OnDisable()
        {
            playerInputActions.Player.Move.started -= OnMoveInputStarted;
            playerInputActions.Player.Move.canceled -= OnMoveInputCanceled;

            playerInputActions.Player.Interact.started -= OnInteractInputStarted;
            playerInputActions.Player.Interact.canceled -= OnInteractInputCanceled;

            playerInputActions.Player.Look.started -= OnLookInputStarted;
            playerInputActions.Player.Look.canceled -= OnLookInputCanceled;

            playerInputActions.Player.Jump.started -= OnJumpInputStarted;
            playerInputActions.Player.Jump.canceled -= OnJumpInputCanceled;

            playerInputActions.Player.Skip.started -= OnSkipInputStarted;
            playerInputActions.Player.Skip.canceled -= OnSkipInputCanceled;

            playerInputActions.Player.Phone.started -= OnPhoneInputStarted;

            playerInputActions.Player.EndDay.started -= OnEndDayInputStarted;

            playerInputActions.Player.Catnip.started -= OnCatnipInputStarted;

            playerInputActions.Player.Scroll.started -= OnScrollInputStarted;

            playerInputActions.Player.Pause.started -= OnPauseInputStarted;

            playerInputActions.Player.Refresh.started -= OnRefreshInputStarted;
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

        private void OnSkipInputStarted(InputAction.CallbackContext context)
        {
            SkipInputStartedEvent?.Invoke();
        }

        private void OnSkipInputCanceled(InputAction.CallbackContext context)
        {
            SkipInputCanceledEvent?.Invoke();
        }

        private void OnPhoneInputStarted(InputAction.CallbackContext context)
        {
            PhoneInputStartedEvent?.Invoke();
        }

        private void OnEndDayInputStarted(InputAction.CallbackContext context)
        {
            EndDayInputStartedEvent?.Invoke();
        }

        private void OnCatnipInputStarted(InputAction.CallbackContext context)
        {
            CatnipInputStartedEvent?.Invoke();
        }

        private void OnCatnipInputCanceled(InputAction.CallbackContext context)
        {
            CatnipInputCanceledEvent?.Invoke();
        }

        private void OnScrollInputStarted(InputAction.CallbackContext context)
        {
            ScrollInputStartedEvent?.Invoke(context.ReadValue<float>());
        }

        private void OnPauseInputStarted(InputAction.CallbackContext context)
        {
            PauseInputStartedEvent?.Invoke();
        }

        private void OnRefreshInputStarted(InputAction.CallbackContext context)
        {
            OnRefreshInputStartedEvent?.Invoke();
        }

        private void OnTimeskipInputStarted(InputAction.CallbackContext context)
        {
            TimeskipInputStartedEvent?.Invoke();
        }
    }
}
