using CardboardCore.DI;
using Grigor.Characters;
using Grigor.Gameplay.Interacting.Components;
using Grigor.UI;
using Grigor.UI.Widgets;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class CatFlapInteractable : InteractableComponent
    {
        // [SerializeField, ColoredBoxGroup("References", false, true)] private new Rigidbody rigidbody;
        // [SerializeField, ColoredBoxGroup("References", false, true)] private LayerMask catLayerMask;
        //
        // [SerializeField, ColoredBoxGroup("Rotation", false, true)] private float rotationForce = 500f;
        // [SerializeField, ColoredBoxGroup("Rotation")] private float maxRotationAngle = 45f;
        // [SerializeField, ColoredBoxGroup("Rotation")] private float bounceForceMultiplier = 0.5f;

        // private Quaternion initialRotation;

        [SerializeField, ColoredBoxGroup("More Meow", false, true)] private bool canFitThroughFlap;
        [SerializeField, ColoredBoxGroup("More Meow"), ShowIf(nameof(canFitThroughFlap))] private Transform enterTeleportTarget;
        [SerializeField, ColoredBoxGroup("More Meow"), ShowIf(nameof(canFitThroughFlap))] private Transform exitTeleportTarget;
        [SerializeField, ColoredBoxGroup("More Meow"), ShowIf(nameof(canFitThroughFlap))] private float teleportDelay = 0.5f;

        [Inject] private UIManager uiManager;
        [Inject] private CharacterRegistry characterRegistry;

        private MessagePopupWidget messagePopupWidget;
        private TransitionWidget transitionWidget;
        private bool wentThroughFlap;

        protected override void OnInitialized()
        {
            // initialRotation = transform.rotation;

            messagePopupWidget = uiManager.GetWidget<MessagePopupWidget>();
            transitionWidget = uiManager.GetWidget<TransitionWidget>();
        }

        private void FixedUpdate()
        {
            // float clampedRotationAngle = Mathf.Clamp(transform.rotation.eulerAngles.x, -maxRotationAngle, maxRotationAngle);
            //
            // transform.rotation = Quaternion.Euler(clampedRotationAngle, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);
        }

        protected override void OnInteractEffect()
        {
            if (canFitThroughFlap)
            {
                Helper.Delay(teleportDelay, TeleportPlayer);

                transitionWidget.Show();

                Helper.Delay(transitionWidget.TransitionDuration, EndInteract);

                return;
            }

            messagePopupWidget.DisplayMessage("You are too big right meow to fit through this gap!");

            EndInteract();
        }

        private void TeleportPlayer()
        {
            Vector3 teleportDestination = wentThroughFlap ? exitTeleportTarget.position : enterTeleportTarget.position;

            characterRegistry.Player.Movement.MovePlayerToPosition(teleportDestination);

            wentThroughFlap = !wentThroughFlap;
        }

        // private void OnCollisionEnter(Collision collision)
        // {
        //     if ((catLayerMask & (1 << collision.gameObject.layer)) != 0)
        //     {
        //         return;
        //     }
        //
        //     SpinFlap(collision.relativeVelocity.magnitude);
        // }
        //
        // private void SpinFlap(float collisionForce)
        // {
        //     Vector3 rotationAxis = transform.right;
        //
        //     float force = collisionForce * rotationForce;
        //
        //     float targetRotation = Mathf.Lerp(0f, maxRotationAngle, Mathf.InverseLerp(0f, rotationForce, force));
        //
        //     rigidbody.AddTorque(rotationAxis * targetRotation, ForceMode.Impulse);
        //
        //     // Bounce back with reduced force
        //     rigidbody.AddForce(-rotationAxis * force * bounceForceMultiplier, ForceMode.Impulse);
        //
        //     // Clamp the rotation angle
        //     float clampedX = Mathf.Clamp(transform.rotation.eulerAngles.x, -maxRotationAngle, maxRotationAngle);
        //     transform.rotation = Quaternion.Euler(clampedX, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);;
        // }
    }
}
