using System;
using CardboardCore.DI;
using Grigor.Input;
using UnityEngine;

namespace Grigor.Characters.Components.Player
{
    public class PlayerLook : CharacterComponent
    {
        [SerializeField] private float mouseSensitivityX;
        [SerializeField] private float mouseSensitivityY;
        [SerializeField] private Transform lookTransform;
        [SerializeField] private Transform lookCameraTransform;
        [SerializeField] private float lookClampX = 85f;

        private float lookRotationX;

        [Inject] private PlayerInput playerInput;

        private Vector2 lookDirection;

        protected override void OnInjected()
        {
            playerInput.LookInputStartedEvent += OnLookInputStarted;
            playerInput.LookInputCanceledEvent += OnLookInputCanceled;
        }

        protected override void OnReleased()
        {
            playerInput.LookInputStartedEvent -= OnLookInputStarted;
            playerInput.LookInputCanceledEvent -= OnLookInputCanceled;
        }

        private void Update()
        {
            MouseLook();
        }

        private void ReadInput()
        {
            lookDirection = playerInput.LookInputDirection;
        }

        private void MouseLook()
        {
            lookTransform.Rotate(Vector3.up, lookDirection.x * mouseSensitivityX * Time.deltaTime);

            lookRotationX -= lookDirection.y * mouseSensitivityY;

            lookRotationX = Mathf.Clamp(lookRotationX, -lookClampX, lookClampX);

            Vector3 targetLookRotation = lookTransform.eulerAngles;

            targetLookRotation.x = lookRotationX;

            lookCameraTransform.eulerAngles = targetLookRotation;
        }

        private void OnLookInputStarted()
        {
            ReadInput();
        }

        private void OnLookInputCanceled()
        {
            lookDirection = Vector2.zero;
        }
    }
}
