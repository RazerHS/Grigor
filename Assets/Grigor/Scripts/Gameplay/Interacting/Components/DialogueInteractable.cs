using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Gameplay.Dialogue;
using Grigor.Input;
using Grigor.Utils;
using Grigor.Utils.StoryGraph.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class DialogueInteractable : InteractableComponent
    {
        [SerializeField] private CharacterData characterData;
        [SerializeField, ValueDropdown(nameof(GetStartNodes))] private string startNodeName;

        [Inject] private DialogueController dialogueController;

        private DialogueNodeData startNode;

        protected override void OnInitialized()
        {
            ValidateDialogueGraph();

            startNode = characterData.CharacterDialogue.ValidateStartNode(startNodeName);
        }

        protected override void OnInteractEffect()
        {
            dialogueController.DialogueEndedEvent += OnDialogueEnded;

            dialogueController.StartDialogue(characterData.CharacterDialogue, startNode);
        }

        private void OnDialogueEnded()
        {
            dialogueController.DialogueEndedEvent -= OnDialogueEnded;

            EndInteract();
        }

        private List<string> GetStartNodes()
        {
            ValidateDialogueGraph();

            return characterData.CharacterDialogue.GetStartNodes();
        }

        private void ValidateDialogueGraph()
        {
            if (characterData == null)
            {
                Log.Error($"Character data in interactable {name} is null!");
            }

            if (characterData.CharacterDialogue == null)
            {
                Log.Error($"Character data in interactable {name} has no dialogue graph set!");
            }
        }

        protected override void OnSkipInputDuringInteraction()
        {
            dialogueController.OnSkipInput();
        }
    }
}
