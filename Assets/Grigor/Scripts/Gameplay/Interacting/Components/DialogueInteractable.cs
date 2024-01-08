using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Gameplay.Dialogue;
using Grigor.Gameplay.Time;
using Grigor.Utils.StoryGraph.Runtime;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class DialogueInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Dialogue", false, true)] private CharacterData characterData;
        [SerializeField, ColoredBoxGroup("Dialogue"), ValueDropdown(nameof(GetStartNodes))] private string startNodeName;
        [SerializeField, ColoredBoxGroup("Dialogue"), Range(0, 60)] private int minutesToPassPerNode = 0;
        [SerializeField, ColoredBoxGroup("Dialogue"), Range(0, 24)] private int hoursToPassPerNode = 0;

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
            dialogueController.NodeEnteredEvent += OnNodeEntered;

            dialogueController.StartDialogue(characterData.CharacterDialogue, startNode);
        }

        private void OnNodeEntered(DialogueNodeData node)
        {
            if (node.NodeType == NodeType.START)
            {
                return;
            }

            TimeManager.PassTime(minutesToPassPerNode, hoursToPassPerNode);
        }

        private void OnDialogueEnded()
        {
            dialogueController.DialogueEndedEvent -= OnDialogueEnded;
            dialogueController.NodeEnteredEvent -= OnNodeEntered;

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
