using CardboardCore.DI;
using Grigor.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Characters.Components.Player
{
    public class PlayerMovement : CharacterComponent
    {
        [Inject] private PlayerInput playerInput;

        [SerializeField] private float movementSpeed;
        [SerializeField] private float gravityStrength = -9.81f;
        [SerializeField] private float jumpHeight;
        [SerializeField] private Transform groundCheckTransform;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform moveTransform;

        [ShowInInspector, ReadOnly] private bool isGrounded;
        private bool jump;
        private bool isMovementEnabled = true;

        private Vector3 moveDirection;
        private Vector3 verticalVelocity;

        private UnityEngine.CharacterController characterController;

        protected override void OnInitialized()
        {
            characterController = GetComponent<UnityEngine.CharacterController>();

            playerInput.MoveInputStartedEvent += OnMoveInputStarted;
            playerInput.MoveInputCanceledEvent += OnMoveInputCanceled;

            playerInput.JumpInputStartedEvent += OnJumpInputStarted;
            playerInput.JumpInputCanceledEvent += OnJumpInputCanceled;

            DisableMovement();
        }

        protected override void OnDisposed()
        {
            playerInput.MoveInputStartedEvent -= OnMoveInputStarted;
            playerInput.MoveInputCanceledEvent -= OnMoveInputCanceled;
        }

        private void Update()
        {
            ReadInput();

            GroundCheck();

            if (!isMovementEnabled && !IsPaused)
            {
                return;
            }

            MovePlayer();
        }

        private void ReadInput()
        {
            moveDirection.x = playerInput.MoveInputDirection.x;
            moveDirection.z = playerInput.MoveInputDirection.y;
        }

        private void GroundCheck()
        {
            isGrounded = Physics.CheckSphere(groundCheckTransform.position, 0.1f, groundMask);

            if (isGrounded)
            {
                verticalVelocity.y = 0;
            }
        }

        private void OnMoveInputStarted()
        {

        }

        private void OnMoveInputCanceled()
        {

        }

        private void OnJumpInputStarted()
        {
            jump = true;
        }

        private void OnJumpInputCanceled()
        {
            jump = false;
        }

        private void MovePlayer()
        {
            Vector3 horizontalVelocity = (moveTransform.right * moveDirection.x + moveTransform.forward * moveDirection.z) * movementSpeed;

            characterController.Move(horizontalVelocity * Time.deltaTime);

            HandleJump();

            verticalVelocity.y += gravityStrength * Time.deltaTime;

            characterController.Move(verticalVelocity * Time.deltaTime);
        }

        private void HandleJump()
        {
            if (!jump)
            {
                return;
            }

            jump = false;

            if (!isGrounded)
            {
                return;
            }

            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityStrength);
        }

        public void EnableMovement()
        {
            isMovementEnabled = true;
        }

        public void DisableMovement()
        {
            isMovementEnabled = false;
        }

        public void MovePlayerTo(Vector3 position)
        {
            transform.position = position;
        }
    }
}
