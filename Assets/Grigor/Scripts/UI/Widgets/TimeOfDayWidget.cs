using CardboardCore.DI;
using Grigor.Gameplay.Time;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Widgets
{
    public class TimeOfDayWidget : UIWidget
    {
        [SerializeField] private TextMeshProUGUI timeText;

        [Inject] private TimeManager timeManager;

        protected override void OnShow()
        {
            timeManager.TimeChangedEvent += OnTimeChanged;
        }

        protected override void OnHide()
        {
            if (timeManager == null)
            {
                return;
            }

            timeManager.TimeChangedEvent -= OnTimeChanged;
        }

        private void OnTimeChanged(int minutes, int hours)
        {
            string minutesString = minutes.ToString();
            string hoursString = hours.ToString();

            if (minutes < 10)
            {
                minutesString = $"0{minutes}";
            }

            if (hours < 10)
            {
                hoursString = $"0{hours}";
            }

            timeText.text = $"{hoursString}:{minutesString}";
        }
    }
}
