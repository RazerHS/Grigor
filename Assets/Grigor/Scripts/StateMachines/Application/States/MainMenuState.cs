using CardboardCore.DI;
using CardboardCore.StateMachines;
using CardboardCore.Utilities;
using Grigor.Input;
using Grigor.UI;
using Grigor.UI.Screens;

namespace Grigor.StateMachines.Application.States
{
    public class MainMenuState : State
    {
        [Inject] private UIManager uiManager;

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

        private void OnSoapInputCanceled()
        {
            Log.Write("alt tab released!");
        }

        private void OnSoapInputStarted()
        {
            Log.Write("alt tab pressed!");
        }

        private void OnPlayButtonPressed()
        {
            owningStateMachine.ToState<GameplayState>();
        }
    }
}
