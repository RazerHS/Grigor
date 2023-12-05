using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Rooms;
using Grigor.Gameplay.Time;
using Grigor.Input;
using Grigor.UI;
using Grigor.UI.Screens;
using Grigor.UI.Widgets;

namespace Grigor.StateMachines.Gameplay.States
{
    public class LevelGameplayState : State
    {
        [Inject] private UIManager uiManager;
        [Inject] private TimeEffectRegistry timeEffectRegistry;
        [Inject] private TimeManager timeManager;
        [Inject] private RoomManager roomManager;
        [Inject] private PlayerInput playerInput;

        private GameplayScreen gameplayScreen;
        private TimeOfDayWidget timeOfDayWidget;
        private DataPodWidget dataPodWidget;

        protected override void OnEnter()
        {
            gameplayScreen = uiManager.ShowScreen<GameplayScreen>();
            timeOfDayWidget = uiManager.GetWidget<TimeOfDayWidget>();
            dataPodWidget = uiManager.GetWidget<DataPodWidget>();

            timeManager.StartTime();

            playerInput.DataPodInputStartedEvent += OnDataPodInputStarted;
            playerInput.EndDayInputStartedEvent += OnEndDayInputStarted;
        }

        private void OnEndDayInputStarted()
        {

        }

        protected override void OnExit()
        {

        }

        private void OnDataPodInputStarted()
        {
            dataPodWidget.OnToggleDataPod();
        }
    }
}
