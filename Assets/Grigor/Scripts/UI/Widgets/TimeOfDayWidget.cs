using CardboardCore.DI;
using Grigor.Gameplay.Time;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class TimeOfDayWidget : UIWidget
    {
        [SerializeField] private Button timeOfDayToggleButton;
        [SerializeField] private float transitionDuration = 3f;

        [Inject] private TimeManager timeManager;

        protected override void OnShow()
        {
            timeOfDayToggleButton.onClick.AddListener(OnTimeOfDayToggleButtonClicked);
        }

        protected override void OnHide()
        {
            timeOfDayToggleButton.onClick.RemoveListener(OnTimeOfDayToggleButtonClicked);
        }

        private void OnTimeOfDayToggleButtonClicked()
        {
            timeOfDayToggleButton.interactable = false;

            timeManager.ToggleTimeOfDay(transitionDuration, EnableButton);
        }

        private void EnableButton()
        {
            timeOfDayToggleButton.interactable = true;
        }

        public void DisableButton()
        {
            timeOfDayToggleButton.interactable = false;
        }
    }
}
