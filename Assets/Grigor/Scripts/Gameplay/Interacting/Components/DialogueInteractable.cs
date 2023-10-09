using Articy.Unity;
using CardboardCore.DI;
using Grigor.Gameplay.Dialogue;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class DialogueInteractable : InteractableComponent
    {
        [SerializeField] private ArticyReference dialogueNode;

        [Inject] private DialogueController dialogueController;

        protected override void OnInteractEffect()
        {
            dialogueController.StartDialogue(dialogueNode.reference.GetObject());

            dialogueController.DialogueEndedEvent += OnDialogueEnded;
        }

        private void OnDialogueEnded()
        {
            dialogueController.DialogueEndedEvent -= OnDialogueEnded;

            EndInteract();
        }
    }
}
