using System;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class EvidenceBoardWidget : UIWidget
    {
        [SerializeField] private Button backButton;

        public event Action OnBackButtonClickedEvent;

        protected override void OnShow()
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        protected override void OnHide()
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        private void OnBackButtonClicked()
        {
            OnBackButtonClickedEvent?.Invoke();
        }
    }
}
