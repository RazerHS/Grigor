using System;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class ToggleMindPalaceWidget : UIWidget
    {
        [SerializeField] private Button button;

        public event Action ToggleMindPalaceEvent;

        protected override void OnShow()
        {
            button.onClick.AddListener(OnToggleMindPalaceClicked);
        }

        protected override void OnHide()
        {
            button.onClick.RemoveListener(OnToggleMindPalaceClicked);
        }

        private void OnToggleMindPalaceClicked()
        {
            ToggleMindPalaceEvent?.Invoke();
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
