using CardboardCore.Utilities;
using Grigor.Gameplay.Clues;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class FindClueInteractable : InteractableComponent
    {
        [SerializeField] private Clue clue;
        [SerializeField] private bool duringNightOnly;

        protected override void OnInteractEffect()
        {
            Log.Write($"Found clue: <b>{clue.ClueData.CredentialType}</b>");

            clue.FindClue();

            EndInteract();
        }
    }
}
