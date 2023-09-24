using CardboardCore.DI;
using CardboardCore.StateMachines;
using CardboardCore.Utilities;
using Grigor.Addressables;
using Grigor.Input;
using Grigor.UI;
using Grigor.UI.Screens;

namespace Grigor.StateMachines.Application.States
{
    public class MainMenuState : State
    {
        [Inject] private UIManager uiManager;
        [Inject] private AddressablesLoader addressablesLoader;

        private MainMenuScreen mainMenuScreen;

        protected override void OnEnter()
        {
            mainMenuScreen = uiManager.ShowScreen<MainMenuScreen>();

            mainMenuScreen.PlayButtonPressedEvent += OnPlayButtonPressed;
        }

        protected override void OnExit()
        {
            mainMenuScreen.PlayButtonPressedEvent -= OnPlayButtonPressed;

            mainMenuScreen.Hide();
        }

        private void OnPlayButtonPressed()
        {
            owningStateMachine.ToState<GameplayState>();
        }
    }
}
