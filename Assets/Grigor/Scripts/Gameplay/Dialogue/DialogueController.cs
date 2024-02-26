using System;
using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Gameplay.Time;
using Grigor.Input;
using Grigor.UI;
using Grigor.UI.Widgets;
using Grigor.Utils.StoryGraph.Runtime;

namespace Grigor.Gameplay.Dialogue
{
    [Injectable]
    public class DialogueController : CardboardCoreBehaviour
    {
        [Inject] private UIManager uiManager;
        [Inject] private TimeManager timeManager;

        private DialogueWidget dialogueWidget;
        private DialogueGraphData currentDialogueGraph;
        private DialogueNodeData currentNode;
        private bool inDialogue;
        private bool currentNodeRequiresChoices;

        public event Action DialogueStartedEvent;
        public event Action DialogueEndedEvent;
        public event Action<DialogueNodeData> NodeEnteredEvent;

        protected override void OnInjected()
        {
            dialogueWidget = uiManager.GetWidget<DialogueWidget>();
        }

        protected override void OnReleased()
        {

        }

        public void OnSkipInput()
        {
            if (!inDialogue)
            {
                return;
            }

            if (currentNodeRequiresChoices)
            {
                return;
            }

            bool lastNode = !currentDialogueGraph.GetNextNode(currentNode, 0, out DialogueNodeData nextNode);

            if (lastNode)
            {
                EndDialogue();

                return;
            }

            OnNextNodeEntered(nextNode);
        }

        private void OnNextNodeEntered(DialogueNodeData nextNode)
        {
            currentNode = nextNode;
            currentNodeRequiresChoices = currentNode.NodeRequiresChoices(out List<DialogueChoiceData> choices);

            UpdateDialogueWidget();

            NodeEnteredEvent?.Invoke(nextNode);

            if (!currentNodeRequiresChoices)
            {
                return;
            }

            foreach (DialogueChoiceData choice in choices)
            {
                dialogueWidget.AddChoice(choice);
            }

            dialogueWidget.ChoiceSelectedEvent += OnChoiceSelected;
        }

        private void OnChoiceSelected(DialogueChoiceData choiceData)
        {
           dialogueWidget.RemoveAllChoices();

           dialogueWidget.ChoiceSelectedEvent -= OnChoiceSelected;

           OnNextNodeEntered(currentDialogueGraph.GetNodeByGuid(choiceData.NextNodeGuid));
        }

        public void StartDialogue(DialogueGraphData dialogueGraphData, DialogueNodeData startNode)
        {
            currentDialogueGraph = dialogueGraphData;

            inDialogue = true;

            OnNextNodeEntered(startNode);

            dialogueWidget.Show();

            DialogueStartedEvent?.Invoke();
        }

        private void EndDialogue()
        {
            currentDialogueGraph = null;

            dialogueWidget.Hide();

            inDialogue = false;

            DialogueEndedEvent?.Invoke();
        }

        private void UpdateDialogueWidget()
        {
            dialogueWidget.SetDialogueText(currentNode.DialogueText);
            dialogueWidget.SetSpeakerText(currentNode.GetSpeakerName());
        }

        public void RefreshCurrentDialogue()
        {
            dialogueWidget.RemoveAllChoices();

            dialogueWidget.ChoiceSelectedEvent -= OnChoiceSelected;

            OnNextNodeEntered(currentDialogueGraph.GetNodeByGuid(currentNode.Guid));
        }
    }
}
