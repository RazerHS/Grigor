using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Utils.StoryGraph.Runtime
{
    [Serializable]
    public class DialogueGraphData : ScriptableObject
    {
        [SerializeField] private List<NodeLinkData> nodeLinks = new();
        [SerializeField] private List<DialogueNodeData> dialogueNodeData = new();
        [SerializeField] private List<ExposedProperty> exposedProperties = new();
        [SerializeField] private List<CommentBlockData> commentBlockData = new();

        public List<NodeLinkData> NodeLinks => nodeLinks;
        public List<DialogueNodeData> DialogueNodeData => dialogueNodeData;
        public List<ExposedProperty> ExposedProperties => exposedProperties;
        public List<CommentBlockData> CommentBlockData => commentBlockData;

        public void SetData(List<NodeLinkData> nodeLinks, List<DialogueNodeData> dialogueNodeData, List<ExposedProperty> exposedProperties, List<CommentBlockData> commentBlockData)
        {
            this.nodeLinks = nodeLinks;
            this.dialogueNodeData = dialogueNodeData;
            this.exposedProperties = exposedProperties;
            this.commentBlockData = commentBlockData;
        }
    }
}
