using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Settings;
using Grigor.UI;
using Grigor.UI.Screens;

namespace Grigor.StateMachines.Application.States
{
    public class SettingsState : State
    {
        [Inject] private UIManager uiManager;
        [Inject] private SettingsManager settingsManager;

        private SettingsScreen settingsScreen;

        protected override void OnEnter()
        {
            settingsScreen = uiManager.ShowScreen<SettingsScreen>();

            settingsScreen.SetResolutionDropdownValues(settingsManager.GetResolutionDropdownOptions());
            settingsScreen.SetQualityDropdownValues(settingsManager.GetQualityDropdownOptions());

            settingsScreen.BackButtonPressedEvent += OnBackButtonPressed;
            settingsScreen.MasterVolumeChangedEvent += OnMasterVolumeChanged;
            settingsScreen.MouseSensitivityChangedEvent += OnMouseSensitivityChanged;
            settingsScreen.ResolutionChangedEvent += OnResolutionChanged;
            settingsScreen.QualityChangedEvent += OnQualityChanged;
        }

        protected override void OnExit()
        {
            settingsScreen.BackButtonPressedEvent -= OnBackButtonPressed;
            settingsScreen.MasterVolumeChangedEvent -= OnMasterVolumeChanged;
            settingsScreen.MouseSensitivityChangedEvent -= OnMouseSensitivityChanged;
            settingsScreen.QualityChangedEvent -= OnQualityChanged;
        }

        private void OnResolutionChanged(int index)
        {
            settingsManager.OnResolutionToggled(index);
        }

        private void OnQualityChanged(int index)
        {
            settingsManager.OnQualityChanged(index);
        }

        private void OnMouseSensitivityChanged(float value)
        {
            settingsManager.OnMouseSensitivityChanged(value);
        }

        private void OnMasterVolumeChanged(float value)
        {
            settingsManager.OnMasterVolumeChanged(value);
        }

        private void OnBackButtonPressed()
        {
            owningStateMachine.ToState<MainMenuState>();
        }
    }
}
