using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Cats;
using Grigor.Gameplay.Time;
using Grigor.Input;
using Grigor.UI;
using Grigor.UI.Widgets;
using UnityEngine;

namespace Grigor.StateMachines.Player.States
{
    public class FreeRoamState : State<PlayerStateMachine>
    {
        [Inject] private UIManager uiManager;
        [Inject] private LevelRegistry levelRegistry;
        [Inject] private TimeManager timeManager;
        [Inject] private PlayerInput playerInput;
        [Inject] private CatManager catManager;

        private PhoneWidget phoneWidget;

        protected override void OnEnter()
        {
            phoneWidget = uiManager.GetWidget<PhoneWidget>();

            owningStateMachine.Owner.Movement.EnableMovement();
            owningStateMachine.Owner.Look.EnableLook();
            owningStateMachine.Owner.Interact.EnableInteract();

            owningStateMachine.Owner.Interact.InteractEvent += OnInteract;

            playerInput.PhoneInputStartedEvent += OnPhoneInputStarted;
            playerInput.CatnipInputStartedEvent += OnCatnipInputStarted;
            playerInput.PauseInputStartedEvent += OnPauseInputStarted;
        }

        protected override void OnExit()
        {
            owningStateMachine.Owner.Movement.DisableMovement();
            owningStateMachine.Owner.Look.DisableLook();
            owningStateMachine.Owner.Interact.DisableInteract();

            owningStateMachine.Owner.Interact.InteractEvent -= OnInteract;

            playerInput.PhoneInputStartedEvent -= OnPhoneInputStarted;
            playerInput.CatnipInputStartedEvent -= OnCatnipInputStarted;
            playerInput.PauseInputStartedEvent -= OnPauseInputStarted;
        }

        private void OnInteract()
        {
            owningStateMachine.ToNextState();
        }

        private void OnPhoneInputStarted()
        {
            phoneWidget.TogglePhone();

            Cursor.visible = !Cursor.visible;
        }

        private void OnCatnipInputStarted()
        {
            if (catManager.IsCatnipPlaced)
            {
                catManager.OnCatnipRemoved();

                return;
            }

            Vector3 catnipPosition = owningStateMachine.Owner.Movement.GroundCheckTransform.position;

            catManager.OnCatnipPlaced(catnipPosition, owningStateMachine.Owner.Look.LookTransform.forward);
        }

        private void OnPauseInputStarted()
        {
            owningStateMachine.ToState<PauseState>();
        }
    }
}
