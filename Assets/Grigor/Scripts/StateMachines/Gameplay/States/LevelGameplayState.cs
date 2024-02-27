using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Time;
using Grigor.Input;
using Grigor.UI;
using Grigor.UI.Screens;
using Grigor.UI.Widgets;
using Grigor.Utils;

namespace Grigor.StateMachines.Gameplay.States
{
    public class LevelGameplayState : State
    {
        [Inject] private UIManager uiManager;
        [Inject] private TimeEffectRegistry timeEffectRegistry;
        [Inject] private TimeManager timeManager;
        [Inject] private LevelRegistry levelRegistry;
        [Inject] private PlayerInput playerInput;

        private GameplayScreen gameplayScreen;
        private TimeOfDayWidget timeOfDayWidget;

        protected override void OnEnter()
        {
            gameplayScreen = uiManager.ShowScreen<GameplayScreen>();

            timeManager.StartTime();

            levelRegistry.CurrentLevel.LevelLighting.WeatherController.Initialize(levelRegistry.CurrentLevel.LevelLighting);

            playerInput.TimeskipInputStartedEvent += OnTimeskipInputStarted;

            Helper.DisableCursor();
        }

        private void OnTimeskipInputStarted()
        {
            timeManager.PassTime(30, 0);
        }

        protected override void OnExit()
        {
            levelRegistry.CurrentLevel.LevelLighting.WeatherController.Dispose();
        }
    }
}
