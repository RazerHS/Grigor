using CardboardCore.DI;
using Grigor.Gameplay.Time;
using Microsoft.Win32.SafeHandles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class TimeOfDayWidget : UIWidget
    {
        [SerializeField] private TextMeshProUGUI timeText;

        [Inject] private TimeManager timeManager;

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        public void UpdateTimeText(int minutes, int hours)
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
