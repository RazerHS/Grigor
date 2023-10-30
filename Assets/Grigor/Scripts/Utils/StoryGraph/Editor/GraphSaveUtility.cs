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
        private DialogueContainer dialogueContainer;
        private StoryGraphView graphView;

        public List<Edge> Edges => graphView.edges.ToList();
        public List<DialogueNode> Nodes => graphView.nodes.ToList().Cast<DialogueNode>().ToList();
        public List<Group> CommentBlocks => graphView.graphElements.ToList().Where(graphElement => graphElement is Group).Cast<Group>().ToList();

        public static GraphSaveUtility GetInstance(StoryGraphView graphView)
        {
            return new GraphSaveUtility
            {
                graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            DialogueContainer dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();

            if (!SaveNodes(fileName, dialogueContainerObject))
            {
                return;
            }

            SaveExposedProperties(dialogueContainerObject);
            SaveCommentBlocks(dialogueContainerObject);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{fileName}.asset", typeof(DialogueContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
			{
                AssetDatabase.CreateAsset(dialogueContainerObject, $"Assets/Resources/{fileName}.asset");
            }
            else
			{
                DialogueContainer container = loadedAsset as DialogueContainer;

                if (container == null)
                {
                    throw Log.Exception($"Loaded asset is not a {nameof(DialogueContainer)}");
                }

                container.NodeLinks = dialogueContainerObject.NodeLinks;
                container.DialogueNodeData = dialogueContainerObject.DialogueNodeData;
                container.ExposedProperties = dialogueContainerObject.ExposedProperties;
                container.CommentBlockData = dialogueContainerObject.CommentBlockData;

                EditorUtility.SetDirty(container);
            }

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(string fileName, DialogueContainer dialogueContainerObject)
        {
            if (!Edges.Any())
            {
                return false;
            }

            Edge[] connectedSockets = Edges.Where(edge => edge.input.node != null).ToArray();

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

                dialogueContainerObject.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.GUID,
                    PortName = connectedSockets[i].output.portName,
                    TargetNodeGuid = inputNode.GUID
                });
            }

            foreach (DialogueNode node in Nodes.Where(node => !node.EntryPoint))
            {
                dialogueContainerObject.DialogueNodeData.Add(new DialogueNodeData
                {
                    NodeGuid = node.GUID,
                    DialogueText = node.DialogueText,
                    Position = node.GetPosition().position
                });
            }

            return true;
        }

        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(graphView.ExposedProperties);
        }

        private void SaveCommentBlocks(DialogueContainer dialogueContainer)
        {
            foreach (Group block in CommentBlocks)
            {
                List<string> nodeGuids = block.containedElements.Where(graphElement => graphElement is DialogueNode).Cast<DialogueNode>().Select(node => node.GUID).ToList();

                dialogueContainer.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodeGuids,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadNarrative(string fileName)
        {
            dialogueContainer = Resources.Load<DialogueContainer>(fileName);

            if (dialogueContainer == null)
            {
                throw Log.Exception($"Dialogue Container with name {fileName} not found!");
            }

            ClearGraph();
            GenerateDialogueNodes();
            ConnectDialogueNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            Nodes.Find(node => node.EntryPoint).GUID = dialogueContainer.NodeLinks[0].BaseNodeGuid;

            foreach (DialogueNode node in Nodes)
            {
                if (node.EntryPoint)
                {
                    continue;
                }

                Edges.Where(edge => edge.input.node == node).ToList().ForEach(edge => graphView.RemoveElement(edge));
                graphView.RemoveElement(node);
            }
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        private void GenerateDialogueNodes()
        {
            foreach (DialogueNodeData nodeData in dialogueContainer.DialogueNodeData)
            {
                DialogueNode tempNode = graphView.CreateNode(nodeData.DialogueText, Vector2.zero);
                tempNode.GUID = nodeData.NodeGuid;

                graphView.AddElement(tempNode);

                List<NodeLinkData> nodePorts = dialogueContainer.NodeLinks.Where(nodeLinkData => nodeLinkData.BaseNodeGuid == nodeData.NodeGuid).ToList();

                nodePorts.ForEach(nodeLinkData => graphView.AddChoicePort(tempNode, nodeLinkData.PortName));
            }
        }

        private void ConnectDialogueNodes()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                int index = i; //Prevent access to modified closure

                List<NodeLinkData> connections = dialogueContainer.NodeLinks.Where(nodeLinkData => nodeLinkData.BaseNodeGuid == Nodes[index].GUID).ToList();

                for (int j = 0; j < connections.Count; j++)
                {
                    string targetNodeGuid = connections[j].TargetNodeGuid;
                    DialogueNode targetNode = Nodes.First(x => x.GUID == targetNodeGuid);

                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(dialogueContainer.DialogueNodeData.First(nodeData => nodeData.NodeGuid == targetNodeGuid).Position, graphView.DefaultNodeSize));
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

        private void AddExposedProperties()
        {
            graphView.ClearBlackboardAndExposedProperties();

            foreach (ExposedProperty exposedProperty in dialogueContainer.ExposedProperties)
            {
                graphView.AddPropertyToBlackboard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks()
        {
            foreach (Group commentBlock in CommentBlocks)
            {
                graphView.RemoveElement(commentBlock);
            }

            foreach (CommentBlockData commentBlockData in dialogueContainer.CommentBlockData)
            {
               Group block = graphView.CreateCommentBlock(new Rect(commentBlockData.Position, graphView.DefaultCommentBlockSize), commentBlockData);

               block.AddElements(Nodes.Where(node => commentBlockData.ChildNodes.Contains(node.GUID)));
            }
        }
    }
}
