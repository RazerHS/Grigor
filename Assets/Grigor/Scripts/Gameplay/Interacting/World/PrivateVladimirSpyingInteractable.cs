using Grigor.Gameplay.Interacting;
using Grigor.Gameplay.Interacting.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class PrivateVladimirSpyingInteractable : DialogueInteractable
    {
        [SerializeField] private Interactable dataInteractable;
        [ShowInInspector] private bool gaveAwayData;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            dataInteractable.InteractEvent += OnGiveAwayData;
        }

        protected override void OnInteractEffect()
        {
            if (!gaveAwayData)
            {
                EndInteract();

                return;
            }

            base.OnInteractEffect();
        }

        private void OnGiveAwayData()
        {
            gaveAwayData = true;
        }
    }
}
