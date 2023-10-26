using CardboardCore.DI;
using CardboardCore.StateMachines;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Gameplay.Rooms;
using Grigor.UI;
using Grigor.UI.Widgets;
using UnityEngine;

namespace Grigor.StateMachines.Player.States
{
    public class MoveToRoomState : State<PlayerStateMachine>
    {
        [Inject] private RoomRegistry roomRegistry;
        [Inject] private CharacterRegistry characterRegistry;
        [Inject] private UIManager uiManager;
        [Inject] private RoomManager roomManager;

        private TransitionWidget transitionWidget;
        private EndDayWidget endDayWidget;

        protected override void OnEnter()
        {
            owningStateMachine.Owner.Movement.DisableMovement();
            owningStateMachine.Owner.Interact.DisableInteract();

            endDayWidget = uiManager.GetWidget<EndDayWidget>();
            endDayWidget.DisableButton();

            transitionWidget = uiManager.ShowWidget<TransitionWidget>();

            transitionWidget.TransitionEndedEvent += OnTransitionEnded;
        }

        private void OnTransitionEnded()
        {
            Room previousRoom = roomRegistry.GetRoom(roomManager.PreviousRoomName);
            Room currentRoom = roomRegistry.GetRoom(roomManager.CurrentRoomName);

            previousRoom.DisableRoom(characterRegistry.Player.Movement.transform.position, true);
            currentRoom.EnableRoom();

            Vector3 position = GetSpawnPosition(previousRoom, currentRoom);

            characterRegistry.Player.Movement.MovePlayerToPosition(position);

            Log.Write($"Moving to room: <b>{currentRoom.RoomName}</b>");

            endDayWidget.EnableButton();

            owningStateMachine.ToState<FreeRoamState>();
        }

        protected override void OnExit()
        {
            transitionWidget.TransitionEndedEvent -= OnTransitionEnded;
        }

        private Vector3 GetSpawnPosition(Room previousRoom, Room currentRoom)
        {
            Vector3 position = currentRoom.GetSpawnPosition();

            if (previousRoom.RoomName == RoomName.MindPalace)
            {
                position = roomManager.PreviousStartRoomPlayerPosition;
            }

            return position;
        }
    }
}
