using CardboardCore.Utilities;
using Grigor.Utils;

namespace Grigor.Overworld.Interacting.Components
{
    public class DialogueInteractable : InteractableComponent
    {
        protected override void OnInteractEffect()
        {
            Log.Write("dialogue");

            Helper.Delay(2f, OnEndDelay);
        }

        private void OnEndDelay()
        {
            EndInteract();
        }
    }
}
