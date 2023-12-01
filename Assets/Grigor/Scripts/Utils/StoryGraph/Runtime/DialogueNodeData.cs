using System;
using Grigor.Characters;
using Grigor.Utils.StoryGraph.Editor.Nodes;
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

        public string NodeName => nodeName;
        public Vector2 Position => position;
        public string DialogueText => dialogueText;
        public CharacterData Speaker => speaker;
        public NodeType NodeType => nodeType;
        public string Guid => guid;

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
    }
}
