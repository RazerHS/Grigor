using System;
using CardboardCore.DI;
using Grigor.Input;
using UnityEngine;

namespace Grigor.Characters.Components.Player.Components
{
    public class PlayerMovement : CharacterComponent
    {
        [Inject] private PlayerInput playerInput;

        [SerializeField] private float movementSpeed;
        [SerializeField] private float movementSmoothTime;

        private Vector3 moveDirection;
        private Vector3 targetTranslationVector;
        private Vector3 currentTranslationVector;
        private Vector3 translationVectorVelocity;
        private bool isMovementEnabled = true;

        private new Rigidbody rigidbody;

        protected override void OnInjected()
        {
            rigidbody = GetComponent<Rigidbody>();

            playerInput.MoveInputStartedEvent += OnMoveInputStarted;
            playerInput.MoveInputCanceledEvent += OnInputDirectionCanceled;
        }

        protected override void OnReleased()
        {
            playerInput.MoveInputStartedEvent -= OnMoveInputStarted;
            playerInput.MoveInputCanceledEvent -= OnInputDirectionCanceled;
        }

        private void FixedUpdate()
        {
            if (!isMovementEnabled && !IsPaused)
            {
                return;
            }

            MovePlayer();
        }

        private void OnInputDirectionCanceled()
        {

        }

        private void OnMoveInputStarted(Vector2 inputDirection)
        {
            moveDirection.x = inputDirection.x;
            moveDirection.z = inputDirection.y;
        }

        private void MovePlayer()
        {
            targetTranslationVector = moveDirection * (movementSpeed * Time.fixedDeltaTime);

            currentTranslationVector = Vector3.SmoothDamp(currentTranslationVector, targetTranslationVector, ref translationVectorVelocity, movementSmoothTime);

            rigidbody.velocity = currentTranslationVector;
        }

        public void EnableMovement()
        {
            isMovementEnabled = true;
        }

        public void DisableMovement()
        {
            isMovementEnabled = false;

            rigidbody.velocity = Vector3.zero;

            OnInputDirectionCanceled();
        }

        public void MovePlayerTo(Vector3 position)
        {
            transform.position = position;
        }
    }
}
