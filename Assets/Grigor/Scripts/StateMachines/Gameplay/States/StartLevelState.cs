using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Characters;
using Grigor.Gameplay.Interacting;
using Grigor.Gameplay.Rooms;
using Grigor.UI;
using Grigor.UI.Widgets;
using UnityEngine;

namespace Grigor.StateMachines.Gameplay.States
{
    public class StartLevelState : State
    {
        [Inject] private RoomRegistry roomRegistry;
        [Inject] private CharacterRegistry characterRegistry;
        [Inject] private InteractablesRegistry interactablesRegistry;
        [Inject] private UIManager uiManager;

        private DataPodWidget dataPodWidget;
        private TimeOfDayToggleWidget timeOfDayToggleWidget;
        private ToggleMindPalaceWidget toggleMindPalaceWidget;

        protected override void OnEnter()
        {
            dataPodWidget = uiManager.ShowWidget<DataPodWidget>();
            timeOfDayToggleWidget = uiManager.ShowWidget<TimeOfDayToggleWidget>();
            toggleMindPalaceWidget = uiManager.ShowWidget<ToggleMindPalaceWidget>();

            interactablesRegistry.EnableInteractables();
            roomRegistry.DisableAllRooms();

            roomRegistry.MovePlayerToRoom(RoomName.Start, characterRegistry.Player);

            characterRegistry.Player.StartStateMachine();

            owningStateMachine.ToNextState();
        }

        protected override void OnExit()
        {

        }
    }
}
