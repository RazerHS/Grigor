using CardboardCore.DI;
using Grigor.Gameplay.Interacting.Components;
using Grigor.UI;
using Grigor.UI.Widgets;
using Grigor.Utils;

namespace Grigor.Gameplay.World.Components
{
    public class BedInteractable : InteractableComponent
    {
        [Inject] private UIManager uiManager;

        private TransitionWidget transitionWidget;

        protected override void OnInitialized()
        {
            transitionWidget = uiManager.GetWidget<TransitionWidget>();
        }

        protected override void OnChangedToDay()
        {
            parentInteractable.PauseInteractable();
        }

        protected override void OnChangedToNight()
        {
            parentInteractable.UnpauseInteractable();
        }

        protected override void OnInteractEffect()
        {
            transitionWidget.Show();

            Helper.Delay(transitionWidget.TransitionDuration, Sleep);
        }

        private void Sleep()
        {
            TimeManager.SetTimeToDay();

            EndInteract();
        }
    }
}
