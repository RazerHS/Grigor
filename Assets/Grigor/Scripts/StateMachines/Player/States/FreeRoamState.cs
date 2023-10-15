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

        private ToggleMindPalaceWidget toggleMindPalaceWidget;

        protected override void OnEnter()
        {
            owningStateMachine.Owner.Movement.EnableMovement();
            owningStateMachine.Owner.Look.EnableLook();
            owningStateMachine.Owner.Interact.EnableInteract();

            toggleMindPalaceWidget = uiManager.GetWidget<ToggleMindPalaceWidget>();

            owningStateMachine.Owner.Interact.InteractEvent += OnInteract;

            owningStateMachine.Owner.Movement.MovePlayerToRoomEvent += OnMovePlayerToRoom;

            toggleMindPalaceWidget.ToggleMindPalaceEvent += OnToggleMindPalace;
        }

        private void OnMovePlayerToRoom(RoomName previousRoomName, RoomName currentRoomName)
        {
            if (owningStateMachine.Owner.Movement.CurrentRoomName == previousRoomName)
            {
                return;
            }

            owningStateMachine.ToState<MoveToRoomState>();
        }

        private void OnToggleMindPalace()
        {
            RoomName nextRoom = owningStateMachine.Owner.Movement.InMindPalace ? RoomName.Start : RoomName.MindPalace;
            owningStateMachine.Owner.Movement.MovePlayerToRoom(nextRoom);
        }

        protected override void OnExit()
        {
            owningStateMachine.Owner.Movement.DisableMovement();
            owningStateMachine.Owner.Look.DisableLook();
            owningStateMachine.Owner.Interact.DisableInteract();

            owningStateMachine.Owner.Interact.InteractEvent -= OnInteract;

            owningStateMachine.Owner.Movement.MovePlayerToRoomEvent -= OnMovePlayerToRoom;

            toggleMindPalaceWidget.ToggleMindPalaceEvent -= OnToggleMindPalace;
        }

        private void OnInteract()
        {
            owningStateMachine.ToNextState();
        }
    }
}
