using Articy.Unity;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Clues;
using Grigor.Gameplay.Dialogue;
using Grigor.Gameplay.Time;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class DialogueInteractable : InteractableComponent
    {
        [SerializeField] private ArticyReference dialogueNode;
        [SerializeField] private Clue clue;

        [Inject] private DialogueController dialogueController;

        protected override void OnInteractEffect()
        {
            dialogueController.StartDialogue(dialogueNode.reference.GetObject());

            dialogueController.DialogueEndedEvent += OnDialogueEnded;
        }

        private void OnDialogueEnded()
        {
            dialogueController.DialogueEndedEvent -= OnDialogueEnded;

            Log.Write($"Found clue: <b>{clue.CredentialToFind}</b>");

            clue.FindClue();

            EndInteract();
        }
    }
}
