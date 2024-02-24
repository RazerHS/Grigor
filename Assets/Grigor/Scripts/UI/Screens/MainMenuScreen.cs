using System;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Screens
{
    public class MainMenuScreen : UIScreen
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;

        public event Action PlayButtonPressedEvent;
        public event Action SettingsButtonPressedEvent;

        protected override void OnShow()
        {
            playButton.onClick.AddListener(OnPlayButtonPressed);
            settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        }

        protected override void OnHide()
        {
            playButton.onClick.RemoveAllListeners();
        }

        private void OnPlayButtonPressed()
        {
            PlayButtonPressedEvent?.Invoke();
        }

        private void OnSettingsButtonPressed()
        {
            SettingsButtonPressedEvent?.Invoke();
        }
    }
}
