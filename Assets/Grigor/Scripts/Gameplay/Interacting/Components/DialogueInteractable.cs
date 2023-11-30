using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Clues;
using Grigor.Gameplay.Dialogue;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class DialogueInteractable : InteractableComponent
    {
        [SerializeField] private Clue clue;

        [Inject] private DialogueController dialogueController;

        protected override void OnInteractEffect()
        {
            dialogueController.DialogueEndedEvent += OnDialogueEnded;
        }

        private void OnDialogueEnded()
        {
            dialogueController.DialogueEndedEvent -= OnDialogueEnded;

            Log.Write($"Found clue: <b>{clue.ClueData.CredentialType}</b>");

            clue.FindClue();

            EndInteract();
        }
    }
}
