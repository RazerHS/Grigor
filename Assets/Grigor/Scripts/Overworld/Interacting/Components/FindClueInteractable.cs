using CardboardCore.Utilities;
using Grigor.Overworld.Clues;
using UnityEngine;

namespace Grigor.Overworld.Interacting.Components
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
