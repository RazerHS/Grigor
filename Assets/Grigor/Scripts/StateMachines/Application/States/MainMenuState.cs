using CardboardCore.DI;
using CardboardCore.StateMachines;
using RazerCore.Utils.Addressables;
using Grigor.UI;
using Grigor.UI.Screens;
using Grigor.Utils;
using UnityEngine;

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
            mainMenuScreen.SettingsButtonPressedEvent += OnSettingsButtonPressed;

            Helper.EnableCursor();
        }

        protected override void OnExit()
        {
            mainMenuScreen.PlayButtonPressedEvent -= OnPlayButtonPressed;
            mainMenuScreen.SettingsButtonPressedEvent -= OnSettingsButtonPressed;

            mainMenuScreen.Hide();
        }

        private void OnPlayButtonPressed()
        {
            owningStateMachine.ToState<GameplayState>();
        }

        private void OnSettingsButtonPressed()
        {
            owningStateMachine.ToState<SettingsState>();
        }
    }
}
