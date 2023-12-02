using System.Collections.Generic;
using System.Linq;
using CardboardCore.Utilities;
using Grigor.Utils.StoryGraph.Editor.Graph;
using Grigor.Utils.StoryGraph.Editor.Nodes;
using Grigor.Utils.StoryGraph.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grigor.Utils.StoryGraph.Editor
{
    public class GraphSaveUtility
    {
        private StoryGraphView graphView;

        private List<Edge> edges => graphView.edges.ToList();
        private List<DialogueNode> nodes => graphView.nodes.ToList().Cast<DialogueNode>().ToList();
        private List<Group> commentBlocks => graphView.graphElements.ToList().Where(graphElement => graphElement is Group).Cast<Group>().ToList();

        public static GraphSaveUtility GetInstance(StoryGraphView graphView)
        {
            return new GraphSaveUtility
            {
                graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            DialogueGraphData newDialogueGraphData = ScriptableObject.CreateInstance<DialogueGraphData>();

            if (!SaveNodes(fileName, newDialogueGraphData))
            {
                return;
            }

            SaveExposedProperties(newDialogueGraphData);
            SaveCommentBlocks(newDialogueGraphData);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{fileName}.asset", typeof(DialogueGraphData));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
			{
                AssetDatabase.CreateAsset(newDialogueGraphData, $"Assets/Resources/{fileName}.asset");
            }
            else
			{
                DialogueGraphData dialogueGraphData = loadedAsset as DialogueGraphData;

                if (dialogueGraphData == null)
                {
                    throw Log.Exception($"Loaded asset is not a {nameof(DialogueGraphData)}");
                }

                dialogueGraphData.SetData(newDialogueGraphData.NodeLinks, newDialogueGraphData.DialogueNodeData, newDialogueGraphData.ExposedProperties, newDialogueGraphData.CommentBlockData);

                EditorUtility.SetDirty(dialogueGraphData);
            }

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(string fileName, DialogueGraphData dialogueGraphData)
        {
            if (!edges.Any())
            {
                return false;
            }

            Edge[] connectedSockets = edges.Where(edge => edge.input.node != null).ToArray();

            for (int i = 0; i < connectedSockets.Length; i++)
            {
                if (connectedSockets[i].input.node is not DialogueNode inputNode)
                {
                    throw Log.Exception("Input node not found!");
                }

                if (connectedSockets[i].output.node is not DialogueNode outputNode)
                {
                    throw Log.Exception("Output node not found!");
                }

                dialogueGraphData.NodeLinks.Add(new NodeLinkData
                {
                    OutputPortName = connectedSockets[i].output.portName,
                    OutputNodeGuid = outputNode.Data.Guid,
                    InputNodeGuid = inputNode.Data.Guid,
                    InputPortName = connectedSockets[i].input.portName,
                });

                DialogueChoiceData choice = outputNode.Data.GetChoiceByText(connectedSockets[i].output.portName);

                choice.SetNextNodeGuid(inputNode.Data.Guid);
            }

            foreach (DialogueNode node in nodes)
            {
                node.Data.SetPosition(node.GetPosition().position);

                DialogueNodeData nodeData = new(node.Data);

                dialogueGraphData.DialogueNodeData.Add(nodeData);
            }

            return true;
        }

        private void SaveExposedProperties(DialogueGraphData dialogueGraphData)
        {
            dialogueGraphData.ExposedProperties.Clear();
            dialogueGraphData.ExposedProperties.AddRange(graphView.ExposedProperties);
        }

        private void SaveCommentBlocks(DialogueGraphData dialogueGraphData)
        {
            foreach (Group block in commentBlocks)
            {
                List<string> nodeGuids = block.containedElements.Where(graphElement => graphElement is DialogueNode).Cast<DialogueNode>().Select(node => node.Data.Guid).ToList();

                dialogueGraphData.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodeGuids,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadGraph(string fileName)
        {
            DialogueGraphData dialogueGraphData = Resources.Load<DialogueGraphData>(fileName);

            if (dialogueGraphData == null)
            {
                throw Log.Exception($"Dialogue Container with name {fileName} not found!");
            }

            ClearGraph();

            GenerateDialogueNodes(dialogueGraphData);
            ConnectDialogueNodes(dialogueGraphData);
            AddExposedProperties(dialogueGraphData);
            GenerateCommentBlocks(dialogueGraphData);
        }

        private void ClearGraph()
        {
            foreach (DialogueNode node in nodes)
            {
                edges.Where(edge => edge.input.node == node).ToList().ForEach(edge => graphView.RemoveElement(edge));
                graphView.RemoveElement(node);
            }
        }

        private void GenerateDialogueNodes(DialogueGraphData dialogueGraphData)
        {
            foreach (DialogueNodeData nodeData in dialogueGraphData.DialogueNodeData)
            {
                DialogueNode newNode = graphView.CreateNode(nodeData.Position, nodeData);

                graphView.AddElement(newNode);
            }
        }

        private void ConnectDialogueNodes(DialogueGraphData dialogueGraphData)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                DialogueNode currentNode = nodes[i];

                List<NodeLinkData> connectionsToCurrentNode = dialogueGraphData.NodeLinks.Where(nodeLink => nodeLink.OutputNodeGuid == currentNode.Data.Guid).ToList();

                for (int j = 0; j < connectionsToCurrentNode.Count; j++)
                {
                    DialogueNode targetNode = nodes.First(node => node.Data.Guid == connectionsToCurrentNode[j].InputNodeGuid);

                    if (targetNode == null)
                    {
                        throw Log.Exception($"Could not find input node with guid {connectionsToCurrentNode[j].InputNodeGuid}!");
                    }

                    LinkNodesTogether(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            Edge edge = new Edge
            {
                output = outputSocket,
                input = inputSocket
            };

            edge.input.Connect(edge);
            edge.output.Connect(edge);

            graphView.Add(edge);
        }

        private void AddExposedProperties(DialogueGraphData dialogueGraphData)
        {
            graphView.ClearBlackboardAndExposedProperties();

            foreach (ExposedProperty exposedProperty in dialogueGraphData.ExposedProperties)
            {
                graphView.AddPropertyToBlackboard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks(DialogueGraphData dialogueGraphData)
        {
            foreach (Group commentBlock in commentBlocks)
            {
                graphView.RemoveElement(commentBlock);
            }

            foreach (CommentBlockData commentBlockData in dialogueGraphData.CommentBlockData)
            {
               Group block = graphView.CreateCommentBlock(new Rect(commentBlockData.Position, graphView.DefaultCommentBlockSize), commentBlockData);

               block.AddElements(nodes.Where(node => commentBlockData.ChildNodes.Contains(node.Data.Guid)));
            }
        }
    }
}
