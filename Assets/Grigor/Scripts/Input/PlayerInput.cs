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

        public event Action SoapInputStartedEvent;
        public event Action SoapInputCanceledEvent;

        private void Awake()
        {
            playerInputActions = new PlayerInputActions();

            playerInputActions.Enable();
        }

        private void OnEnable()
        {
            playerInputActions.Player.Soap.started += OnSoapInputStarted;
            playerInputActions.Player.Soap.canceled += OnSoapInputCanceled;
        }

        private void OnSoapInputStarted(InputAction.CallbackContext context)
        {
            SoapInputStartedEvent?.Invoke();
        }

        private void OnSoapInputCanceled(InputAction.CallbackContext context)
        {
            SoapInputCanceledEvent?.Invoke();
        }
    }
}
