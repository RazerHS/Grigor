using CardboardCore.DI;
using Grigor.Gameplay.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class ToggleMindPalaceWidget : UIWidget
    {
        [SerializeField] private Button button;

        [Inject] private MindPalaceManager mindPalaceManager;

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
            if (mindPalaceManager.InsideMindPalace)
            {
                mindPalaceManager.ExitMindPalace();
                return;
            }

            mindPalaceManager.EnterMindPalace();
        }
    }
}
