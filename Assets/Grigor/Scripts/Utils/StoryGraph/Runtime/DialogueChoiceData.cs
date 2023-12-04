using System;
using UnityEngine;

namespace Grigor.Utils.StoryGraph.Runtime
{
    [Serializable]
    public class DialogueChoiceData
    {
        [SerializeField] private string text;
        [SerializeField] private string nextNodeGuid;

        public string Text => text;
        public string NextNodeGuid => nextNodeGuid;

        public void SetChoiceText(string choiceText)
        {
            this.text = choiceText;
        }

        public void SetNextNodeGuid(string nextNodeGuid)
        {
            this.nextNodeGuid = nextNodeGuid;
        }
    }
}
