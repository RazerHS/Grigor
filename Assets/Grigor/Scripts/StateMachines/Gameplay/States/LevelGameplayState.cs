using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.UI;
using Grigor.UI.Screens;

namespace Grigor.StateMachines.Gameplay.States
{
    public class LevelGameplayState : State
    {
        [Inject] private UIManager uiManager;

        private GameplayScreen gameplayScreen;

        protected override void OnEnter()
        {
            gameplayScreen = uiManager.ShowScreen<GameplayScreen>();
        }

        protected override void OnExit()
        {

        }
    }
}
