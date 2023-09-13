using System;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Screens
{
    public class MainMenuScreen : UIScreen
    {
        [SerializeField] private Button playButton;

        public event Action PlayButtonPressedEvent;

        protected override void OnShow()
        {
            playButton.onClick.AddListener(OnPlayButtonPressed);
        }

        protected override void OnHide()
        {
            playButton.onClick.RemoveListener(OnPlayButtonPressed);
        }

        private void OnPlayButtonPressed()
        {
            PlayButtonPressedEvent?.Invoke();
        }
    }
}
