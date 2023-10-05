using CardboardCore.DI;
using Grigor.Gameplay.Time;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class TimeOfDayToggleWidget : UIWidget
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

            timeManager.ToggleTimeOfDay(transitionDuration, EnableToggleButton);
        }

        private void EnableToggleButton()
        {
            timeOfDayToggleButton.interactable = true;
        }
    }
}
