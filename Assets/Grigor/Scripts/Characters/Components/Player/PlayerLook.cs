using CardboardCore.DI;
using Grigor.Input;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Characters.Components.Player
{
    public class PlayerLook : CharacterComponent
    {
        [SerializeField, ColoredBoxGroup("Values", false, 0.5f, 0.1f, 0.1f)] private float mouseSensitivityX;
        [SerializeField, ColoredBoxGroup("Values")] private float mouseSensitivityY;
        [SerializeField, ColoredBoxGroup("Values")] private float lookClampX = 85f;
        [SerializeField, ColoredBoxGroup("References", false, 0.1f, 0.1f, 0.9f)] private Transform lookTransform;
        [SerializeField, ColoredBoxGroup("References")] private Transform lookCameraTransform;

        [Inject] private PlayerInput playerInput;

        private float lookRotationX;
        private Vector2 lookDirection;
        private bool lookEnabled;

        public Transform LookTransform => lookTransform;

        protected override void OnInitialized()
        {
            playerInput.LookInputStartedEvent += OnLookInputStarted;
            playerInput.LookInputCanceledEvent += OnLookInputCanceled;
        }

        protected override void OnDisposed()
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
            if (!lookEnabled)
            {
                return;
            }

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

        public void EnableLook()
        {
            lookEnabled = true;
        }

        public void DisableLook()
        {
            lookEnabled = false;
        }
    }
}
