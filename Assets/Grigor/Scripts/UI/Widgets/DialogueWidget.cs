using System;
using System.Collections.Generic;
using Grigor.UI.Data;
using Grigor.Utils.StoryGraph.Runtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Widgets
{
    public class DialogueWidget : UIWidget
    {
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private Transform choiceParent;
        [SerializeField] private DialogueChoiceUIDisplay dialogueChoiceDisplayPrefab;

        [ShowInInspector, ReadOnly] private List<DialogueChoiceUIDisplay> currentChoices = new();

        public event Action<DialogueChoiceData> ChoiceSelectedEvent;

        protected override void OnShow()
        {

        }

        protected override void OnHide()
        {

        }

        public void SetDialogueText(string text)
        {
            dialogueText.text = text;
        }

        public void SetSpeakerText(string text)
        {
            speakerText.text = text;
        }

        public void AddChoice(DialogueChoiceData choiceData)
        {
            DialogueChoiceUIDisplay dialogueChoiceUIDisplay = Instantiate(dialogueChoiceDisplayPrefab, choiceParent);

            currentChoices.Add(dialogueChoiceUIDisplay);

            dialogueChoiceUIDisplay.Initialize(choiceData);

            dialogueChoiceUIDisplay.ChoiceSelectedEvent += OnChoiceSelected;
        }

        public void RemoveAllChoices()
        {
            for (int i = currentChoices.Count - 1; i >= 0; i--)
            {
                currentChoices[i].ChoiceSelectedEvent -= OnChoiceSelected;

                currentChoices[i].Dispose();

                currentChoices.RemoveAt(i);
            }
        }

        private void OnChoiceSelected(DialogueChoiceData choiceData)
        {
            ChoiceSelectedEvent?.Invoke(choiceData);
        }
    }
}
