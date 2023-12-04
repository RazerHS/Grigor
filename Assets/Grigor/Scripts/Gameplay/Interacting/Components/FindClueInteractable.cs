using System;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data.Clues;
using Grigor.Gameplay.Clues;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class FindClueInteractable : InteractableComponent
    {
        [SerializeField] private ClueData clueToFind;

        [Inject] private ClueRegistry clueRegistry;

        protected override void OnInitialized()
        {

            if (clueToFind == null)
            {
                throw Log.Exception($"Clue to find not set in interactable {name}!");
            }
            clueRegistry.RegisterClue(clueToFind);
        }

        protected override void OnInteractEffect()
        {
            Log.Write($"Found clue: <b>{clueToFind.CredentialType}</b>");

            clueToFind.OnClueFound();

            clueRegistry.UnregisterClue(clueToFind);

            EndInteract();
        }
    }
}
