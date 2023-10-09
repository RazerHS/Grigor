using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class ToggleMindPalaceWidget : UIWidget
    {
        [SerializeField] private Button button;

        protected override void OnShow()
        {
            button.onClick.AddListener(OnToggleMindPalaceClicked);
        }

        protected override void OnHide()
        {
            throw new System.NotImplementedException();
        }

        private void OnToggleMindPalaceClicked()
        {
            throw new System.NotImplementedException();
        }
    }
}
