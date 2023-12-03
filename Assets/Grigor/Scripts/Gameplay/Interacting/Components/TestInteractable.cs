namespace Grigor.Gameplay.Interacting.Components
{
    public class TestInteractable : InteractableComponent
    {
        protected override void OnInteractEffect()
        {
            EndInteract();
        }
    }
}
