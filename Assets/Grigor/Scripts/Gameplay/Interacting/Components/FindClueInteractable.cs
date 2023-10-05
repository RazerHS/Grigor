using CardboardCore.Utilities;
using Grigor.Gameplay.Clues;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class FindClueInteractable : InteractableComponent
    {
        [SerializeField] private Clue clue;

        protected override void OnInteractEffect()
        {
            Log.Write($"found clue: <b>{clue.CredentialToFind}</b>");

            clue.FindClue();

            EndInteract();
        }
    }
}
