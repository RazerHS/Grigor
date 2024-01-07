using CardboardCore.DI;
using Grigor.Characters;
using Grigor.Gameplay.Interacting.Components;
using Grigor.Gameplay.Time;
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
        [SerializeField, ColoredBoxGroup("More Meow", false, true)] private bool canFitThroughFlap;
        [SerializeField, ColoredBoxGroup("More Meow/Telepawrting", false, true), ShowIf(nameof(canFitThroughFlap))] private Transform enterTeleportTarget;
        [SerializeField, ColoredBoxGroup("More Meow/Telepawrting"), ShowIf(nameof(canFitThroughFlap))] private Transform exitTeleportTarget;
        [SerializeField, ColoredBoxGroup("More Meow/Telepawrting"), ShowIf(nameof(canFitThroughFlap))] private float teleportDelay = 0.5f;
        [SerializeField, ColoredBoxGroup("More Meow/Flying Flap", false, true), ShowIf(nameof(canFitThroughFlap))] private CollisionData collisionData;
        [SerializeField, ColoredBoxGroup("More Meow/Flying Flap"), ShowIf(nameof(canFitThroughFlap))] private Rigidbody flapRigidbody;
        [SerializeField, ColoredBoxGroup("More Meow/Flying Flap"), ShowIf(nameof(canFitThroughFlap))] private Vector3 flapForce = new Vector3(5f, 25f, 5f);
        [SerializeField, ColoredBoxGroup("More Meow/Flying Flap"), ShowIf(nameof(canFitThroughFlap))] private Vector3 flapTorque = new Vector3(5f, 5f, 5f);

        [ShowInInspector, ColoredBoxGroup("More Meow/Flying Flap"), ShowIf(nameof(canFitThroughFlap)), Button(ButtonSizes.Large)] private void ResetFlap() => OnResetFlap();

        [Inject] private UIManager uiManager;
        [Inject] private CharacterRegistry characterRegistry;

        private MessagePopupWidget messagePopupWidget;
        private TransitionWidget transitionWidget;
        private bool wentThroughFlap;

        private Transform flapTransform;
        private Vector3 initialFlapPosition;
        private Quaternion initialFlapRotation;

        protected override void OnInitialized()
        {
            messagePopupWidget = uiManager.GetWidget<MessagePopupWidget>();
            transitionWidget = uiManager.GetWidget<TransitionWidget>();

            collisionData.CollisionEvent += OnCollisionWithCat;

            flapTransform = flapRigidbody.transform;

            initialFlapPosition = flapTransform.position;
            initialFlapRotation = flapTransform.rotation;
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

        private void OnCollisionWithCat(Collision collision)
        {
            if (!canFitThroughFlap)
            {
                return;
            }

            flapRigidbody.constraints = RigidbodyConstraints.None;

            flapRigidbody.AddForce(collision.impulse + flapForce, ForceMode.Impulse);
            flapRigidbody.AddTorque(flapTorque, ForceMode.Impulse);
        }

        private void OnResetFlap()
        {
            flapTransform.position = initialFlapPosition;

            flapRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            flapRigidbody.constraints &= RigidbodyConstraints.FreezeRotationX;
        }
    }
}
