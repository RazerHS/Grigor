using System;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class EndDayWidget : UIWidget
    {
        [SerializeField] private Button button;

        public event Action DayEndedEvent;

        protected override void OnShow()
        {
            button.onClick.AddListener(OnEndDayClicked);
        }

        protected override void OnHide()
        {
            button.onClick.RemoveListener(OnEndDayClicked);
        }

        private void OnEndDayClicked()
        {
            DayEndedEvent?.Invoke();
        }

        public void EnableButton()
        {
            button.interactable = true;
        }

        public void DisableButton()
        {
            button.interactable = false;
        }
    }
}
