using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Rooms;
using Grigor.Gameplay.Time;
using Grigor.Input;
using Grigor.UI;
using UnityEngine;

namespace Grigor.StateMachines.Player.States
{
    public class FreeRoamState : State<PlayerStateMachine>
    {
        [Inject] private UIManager uiManager;
        [Inject] private RoomRegistry roomRegistry;
        [Inject] private RoomManager roomManager;
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

            roomManager.MovePlayerToRoomEvent += OnMovePlayerToRoom;

            playerInput.DataPodInputStartedEvent += OnDataPodInputStarted;
            playerInput.EndDayInputStartedEvent += OnEndedDayInput;
        }

        protected override void OnExit()
        {
            owningStateMachine.Owner.Movement.DisableMovement();
            owningStateMachine.Owner.Look.DisableLook();
            owningStateMachine.Owner.Interact.DisableInteract();

            owningStateMachine.Owner.Interact.InteractEvent -= OnInteract;

            roomManager.MovePlayerToRoomEvent -= OnMovePlayerToRoom;

            playerInput.DataPodInputStartedEvent += OnDataPodInputStarted;
            playerInput.EndDayInputStartedEvent += OnEndedDayInput;
        }

        private void OnMovePlayerToRoom(RoomName previousRoomName, RoomName currentRoomName)
        {
            if (roomManager.CurrentRoomName == previousRoomName)
            {
                return;
            }

            owningStateMachine.ToState<MoveToRoomState>();
        }

        private void OnEndedDayInput()
        {
            if (!timeManager.TryEndDay())
            {
                return;
            }

            roomManager.MovePlayerToRoom(RoomName.MindPalace, owningStateMachine.Owner.transform.position);
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
