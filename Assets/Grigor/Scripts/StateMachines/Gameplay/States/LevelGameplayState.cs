using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Rooms;
using Grigor.Gameplay.Time;
using Grigor.UI;
using Grigor.UI.Screens;
using Grigor.UI.Widgets;

namespace Grigor.StateMachines.Gameplay.States
{
    public class LevelGameplayState : State, ITimeEffect
    {
        [Inject] private UIManager uiManager;
        [Inject] private TimeEffectRegistry timeEffectRegistry;
        [Inject] private TimeManager timeManager;
        [Inject] private RoomManager roomManager;

        private GameplayScreen gameplayScreen;
        private EndDayWidget endDayWidget;

        protected override void OnEnter()
        {
            gameplayScreen = uiManager.ShowScreen<GameplayScreen>();
            endDayWidget = uiManager.GetWidget<EndDayWidget>();

            RegisterTimeEffect();

            timeManager.StartTime();
        }

        protected override void OnExit()
        {

        }

        public void RegisterTimeEffect()
        {
            timeEffectRegistry.Register(this);
        }

        public void OnChangedToDay()
        {
            endDayWidget.DisableButton();
        }

        public void OnChangedToNight()
        {
            endDayWidget.EnableButton();
        }
    }
}
