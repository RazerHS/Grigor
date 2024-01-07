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
            messagePopupWidget = uiManager.GetWidget<MessagePopupWidget>();
            transitionWidget = uiManager.GetWidget<TransitionWidget>();
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
    }
}
