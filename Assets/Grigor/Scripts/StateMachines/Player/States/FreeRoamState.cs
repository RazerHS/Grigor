using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Rooms;
using Grigor.UI;
using Grigor.UI.Widgets;

namespace Grigor.StateMachines.Player.States
{
    public class FreeRoamState : State<PlayerStateMachine>
    {
        [Inject] private UIManager uiManager;
        [Inject] private RoomRegistry roomRegistry;
        [Inject] private RoomManager roomManager;

        private EndDayWidget endDayWidget;
        private TimeOfDayWidget timeOfDayWidget;

        protected override void OnEnter()
        {
            owningStateMachine.Owner.Movement.EnableMovement();
            owningStateMachine.Owner.Look.EnableLook();
            owningStateMachine.Owner.Interact.EnableInteract();

            endDayWidget = uiManager.GetWidget<EndDayWidget>();
            timeOfDayWidget = uiManager.GetWidget<TimeOfDayWidget>();

            owningStateMachine.Owner.Interact.InteractEvent += OnInteract;

            roomManager.MovePlayerToRoomEvent += OnMovePlayerToRoom;

            endDayWidget.DayEndedEvent += OnDayEnded;

            if (roomManager.CurrentRoomName == RoomName.Start)
            {
                OnDayStarted();
            }
        }

        private void OnMovePlayerToRoom(RoomName previousRoomName, RoomName currentRoomName)
        {
            if (roomManager.CurrentRoomName == previousRoomName)
            {
                return;
            }

            owningStateMachine.ToState<MoveToRoomState>();
        }

        private void OnDayEnded()
        {
            endDayWidget.DisableButton();

            RoomName nextRoom = roomManager.PlayerInMindPalace ? RoomName.Start : RoomName.MindPalace;
            roomManager.MovePlayerToRoom(nextRoom, owningStateMachine.Owner.transform.position);
        }

        private void OnDayStarted()
        {
            if (roomManager.CurrentRoomName != RoomName.MindPalace)
            {
                endDayWidget.EnableButton();
            }
        }

        protected override void OnExit()
        {
            owningStateMachine.Owner.Movement.DisableMovement();
            owningStateMachine.Owner.Look.DisableLook();
            owningStateMachine.Owner.Interact.DisableInteract();

            owningStateMachine.Owner.Interact.InteractEvent -= OnInteract;

            roomManager.MovePlayerToRoomEvent -= OnMovePlayerToRoom;

            endDayWidget.DayEndedEvent -= OnDayEnded;
        }

        private void OnInteract()
        {
            owningStateMachine.ToNextState();
        }
    }
}
