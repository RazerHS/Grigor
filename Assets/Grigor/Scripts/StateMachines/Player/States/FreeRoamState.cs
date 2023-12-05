using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Rooms;
using Grigor.Gameplay.Time;
using Grigor.Input;
using Grigor.UI;

namespace Grigor.StateMachines.Player.States
{
    public class FreeRoamState : State<PlayerStateMachine>
    {
        [Inject] private UIManager uiManager;
        [Inject] private RoomRegistry roomRegistry;
        [Inject] private RoomManager roomManager;
        [Inject] private TimeManager timeManager;
        [Inject] private PlayerInput playerInput;

        protected override void OnEnter()
        {
            owningStateMachine.Owner.Movement.EnableMovement();
            owningStateMachine.Owner.Look.EnableLook();
            owningStateMachine.Owner.Interact.EnableInteract();

            owningStateMachine.Owner.Interact.InteractEvent += OnInteract;

            roomManager.MovePlayerToRoomEvent += OnMovePlayerToRoom;
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

            timeManager.SetTimeToNight();

            RoomName nextRoom = roomManager.PlayerInMindPalace ? RoomName.Start : RoomName.MindPalace;
            roomManager.MovePlayerToRoom(nextRoom, owningStateMachine.Owner.transform.position);
        }

        private void OnDayStarted()
        {
            timeManager.SetTimeToDay();
        }

        protected override void OnExit()
        {
            owningStateMachine.Owner.Movement.DisableMovement();
            owningStateMachine.Owner.Look.DisableLook();
            owningStateMachine.Owner.Interact.DisableInteract();

            owningStateMachine.Owner.Interact.InteractEvent -= OnInteract;

            roomManager.MovePlayerToRoomEvent -= OnMovePlayerToRoom;
        }

        private void OnInteract()
        {
            owningStateMachine.ToNextState();
        }
    }
}
