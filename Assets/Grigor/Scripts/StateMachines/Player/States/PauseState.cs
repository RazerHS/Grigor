using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Settings;
using Grigor.UI;
using Grigor.UI.Widgets;
using Grigor.Utils;
using UnityEngine;

namespace Grigor.StateMachines.Player.States
{
    public class PauseState : State
    {
        [Inject] private UIManager uiManager;
        [Inject] private SettingsManager settingsManager;

        private PauseMenuWidget pauseMenuWidget;

        protected override void OnEnter()
        {
            pauseMenuWidget = uiManager.ShowWidget<PauseMenuWidget>();

            pauseMenuWidget.InitializeDropdownOptions(settingsManager.GetResolutionDropdownOptions(), settingsManager.GetQualityDropdownOptions());

            pauseMenuWidget.ForceSetResolutionValueInDropdown(settingsManager.CurrentResolutionIndex);
            pauseMenuWidget.ForceSetQualityValueInDropdown(settingsManager.CurrentQualityIndex);

            pauseMenuWidget.BackButtonPressedEvent += OnBackButtonPressed;
            pauseMenuWidget.MasterVolumeChangedEvent += OnMasterVolumeChanged;
            pauseMenuWidget.MouseSensitivityChangedEvent += OnMouseSensitivityChanged;
            pauseMenuWidget.ResolutionChangedEvent += OnResolutionChanged;
            pauseMenuWidget.QualityChangedEvent += OnQualityChanged;

            Helper.EnableCursor();
        }

        protected override void OnExit()
        {
            pauseMenuWidget.BackButtonPressedEvent -= OnBackButtonPressed;
            pauseMenuWidget.MasterVolumeChangedEvent -= OnMasterVolumeChanged;
            pauseMenuWidget.MouseSensitivityChangedEvent -= OnMouseSensitivityChanged;
            pauseMenuWidget.QualityChangedEvent -= OnQualityChanged;

            pauseMenuWidget.Hide();
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
            owningStateMachine.ToState<FreeRoamState>();
        }
    }
}
