using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Characters;
using Grigor.Gameplay.Interacting;
using Grigor.Gameplay.Rooms;
using Grigor.Gameplay.Time;
using Grigor.UI;
using Grigor.UI.Widgets;

namespace Grigor.StateMachines.Gameplay.States
{
    public class StartLevelState : State
    {
        [Inject] private RoomRegistry roomRegistry;
        [Inject] private CharacterRegistry characterRegistry;
        [Inject] private InteractablesRegistry interactablesRegistry;
        [Inject] private UIManager uiManager;
        [Inject] private TimeManager timeManager;

        private DataPodWidget dataPodWidget;
        private TimeOfDayWidget timeOfDayWidget;
        private EndDayWidget endDayWidget;

        protected override void OnEnter()
        {
            characterRegistry.Player.StartStateMachine();

            dataPodWidget = uiManager.ShowWidget<DataPodWidget>();
            timeOfDayWidget = uiManager.ShowWidget<TimeOfDayWidget>();
            endDayWidget = uiManager.ShowWidget<EndDayWidget>();

            interactablesRegistry.EnableInteractables();
            roomRegistry.DisableAllRooms();

            Room startRoom = roomRegistry.GetRoom(RoomName.Start);
            startRoom.EnableRoom();

            characterRegistry.Player.Movement.MovePlayerToPosition(startRoom.SpawnPoint.position);

            owningStateMachine.ToNextState();
        }

        protected override void OnExit()
        {

        }
    }
}
