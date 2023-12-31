using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Time;
using Grigor.Input;
using Grigor.UI;
using UnityEngine;

namespace Grigor.StateMachines.Player.States
{
    public class FreeRoamState : State<PlayerStateMachine>
    {
        [Inject] private UIManager uiManager;
        [Inject] private LevelRegistry levelRegistry;
        [Inject] private TimeManager timeManager;
        [Inject] private PlayerInput playerInput;

        private DataPodWidget dataPodWidget;

        protected override void OnEnter()
        {
            dataPodWidget = uiManager.GetWidget<DataPodWidget>();

            owningStateMachine.Owner.Movement.EnableMovement();
            owningStateMachine.Owner.Look.EnableLook();
            owningStateMachine.Owner.Interact.EnableInteract();

            owningStateMachine.Owner.Interact.InteractEvent += OnInteract;

            playerInput.DataPodInputStartedEvent += OnDataPodInputStarted;
        }

        protected override void OnExit()
        {
            owningStateMachine.Owner.Movement.DisableMovement();
            owningStateMachine.Owner.Look.DisableLook();
            owningStateMachine.Owner.Interact.DisableInteract();

            owningStateMachine.Owner.Interact.InteractEvent -= OnInteract;

            playerInput.DataPodInputStartedEvent += OnDataPodInputStarted;
        }

        private void OnInteract()
        {
            owningStateMachine.ToNextState();
        }

        private void OnDataPodInputStarted()
        {
            dataPodWidget.OnToggleDataPod();

            Cursor.visible = !Cursor.visible;
        }
    }
}
