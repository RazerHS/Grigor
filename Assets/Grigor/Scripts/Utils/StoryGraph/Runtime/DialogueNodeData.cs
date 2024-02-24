using System;
using System.Collections.Generic;
using Grigor.Characters;
using Sirenix.Utilities;
using UnityEngine;

namespace Grigor.Utils.StoryGraph.Runtime
{
    [Serializable]
    public class DialogueNodeData
    {
        [SerializeField] private string nodeName;
        [SerializeField] private string guid;
        [SerializeField] private Vector2 position;
        [SerializeField] private string dialogueText;
        [SerializeField] private CharacterData speaker;
        [SerializeField] private NodeType nodeType;
        [SerializeField] private List<DialogueChoiceData> choices = new();

        public string NodeName => nodeName;
        public Vector2 Position => position;
        public string DialogueText => dialogueText;
        public CharacterData Speaker => speaker;
        public NodeType NodeType => nodeType;
        public string Guid => guid;
        public List<DialogueChoiceData> Choices => choices;

        public DialogueNodeData()
        {

        }

        public DialogueNodeData(DialogueNodeData data)
        {
            nodeName = data.nodeName;
            guid = data.guid;
            position = data.position;
            dialogueText = data.dialogueText;
            speaker = data.speaker;
            nodeType = data.nodeType;
            choices = data.choices;
        }

        public void SetNodeName(string nodeName)
        {
            this.nodeName = nodeName;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public void SetDialogueText(string dialogueText)
        {
            this.dialogueText = dialogueText;
        }

        public void SetSpeaker(CharacterData speaker)
        {
            this.speaker = speaker;
        }

        public void SetNodeType(NodeType nodeType)
        {
            this.nodeType = nodeType;
        }

        public void SetGuid(string guid)
        {
            this.guid = guid;
        }

        public void AddChoice(DialogueChoiceData choice)
        {
            if (choices.Contains(choice))
            {
                return;
            }

            choices.Add(choice);
        }

        public void RemoveChoice(DialogueChoiceData choice)
        {
            if (!choices.Contains(choice))
            {
                return;
            }

            choices.Remove(choice);
        }

        public DialogueChoiceData GetChoiceByText(string text)
        {
            foreach (DialogueChoiceData choice in choices)
            {
                if (choice.Text != text)
                {
                    continue;
                }

                return choice;
            }

            throw new Exception($"Choice with text {text} not found!");
        }

        public void RemoveChoiceByChoiceText(string text)
        {
            foreach (DialogueChoiceData choice in choices)
            {
                if (choice.Text != text)
                {
                    continue;
                }

                choices.Remove(choice);

                return;
            }
        }

        public void RemoveAllChoices()
        {
            choices.Clear();
        }

        public bool NodeRequiresChoices(out List<DialogueChoiceData> newChoices)
        {
            newChoices = choices;

            if (choices.Count > 1)
            {
                return true;
            }

            if (choices.IsNullOrEmpty())
            {
                return false;
            }

            return choices[0].Text != "1";
        }

        public DialogueChoiceData GetChoiceByIndex(int index)
        {
            return choices[index];
        }

        public bool DoOverridenChoicesExist()
        {
            if (choices.Count == 1)
            {
                //default port name
                return choices[0].Text != "1";
            }

            return true;
        }

        public string GetSpeakerName()
        {
            return speaker == null ? "None" : speaker.name;
        }
    }
}
