using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using CardboardCore.Utilities;
using Grigor.Utils.StoryGraph.Editor.Nodes;
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

        public bool GetNextNode(DialogueNodeData currentNodeData, int outputPortChoiceIndex, out DialogueNodeData nodeData)
        {
            nodeData = null;

            if (currentNodeData.NodeType == NodeType.END)
            {
                return false;
            }

            NodeLinkData nodeLinkData = nodeLinks.Find(nodeLink => nodeLink.OutputNodeGuid == currentNodeData.Guid && nodeLink.OutportPortChoiceIndex == outputPortChoiceIndex);

            if (nodeLinkData == null)
            {
                throw Log.Exception($"Node link data not found for <b>{currentNodeData.Guid}</b> and <b>{outputPortChoiceIndex}</b>!");
            }

            nodeData = dialogueNodeData.Find(dialogueNode => dialogueNode.Guid == nodeLinkData.InputNodeGuid);

            return true;
        }

        public DialogueNodeData GetStartNodeByName(string nodeName)
        {
            DialogueNodeData startNodeData = dialogueNodeData.Find(dialogueNode => dialogueNode.NodeName == nodeName);

            if (startNodeData.NodeType != NodeType.START)
            {
                throw Log.Exception($"Node <b>{nodeName}</b> is not a start node!");
            }

            return startNodeData;
        }

        public List<string> GetStartNodes()
        {
            return dialogueNodeData.Where(node => node.NodeType == NodeType.START).Select(node => node.NodeName).ToList();
        }

        public DialogueNodeData ValidateStartNode(string startNodeName)
        {
            if (string.IsNullOrEmpty(startNodeName))
            {
                throw Log.Exception("Start node name is empty!");
            }

            DialogueNodeData startNodeData = dialogueNodeData.Find(dialogueNode => dialogueNode.NodeName == startNodeName);

            if (startNodeData == null)
            {
                throw Log.Exception($"Start node <b>{startNodeName}</b> not found!");
            }

            if (startNodeData.NodeType != NodeType.START)
            {
                throw Log.Exception($"Node <b>{startNodeName}</b> is not a start node!");
            }

            return startNodeData;
        }
    }
}
