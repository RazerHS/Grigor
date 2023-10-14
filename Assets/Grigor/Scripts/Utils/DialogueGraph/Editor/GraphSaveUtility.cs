using System.Collections.Generic;
using System.Linq;
using RazerCore.Utils.DialogueGraph.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RazerCore.Utils.DialogueGraph.Editor
{
    public class GraphSaveUtility
    {
        private DialogueGraphView graphView;
        private DialogueContainer dialogueContainer;

        private List<Edge> edges => graphView.edges.ToList();
        private List<DialogueNode> nodes => graphView.nodes.ToList().Cast<DialogueNode>().ToList();

        public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
        {
            return new GraphSaveUtility
            {
                graphView = targetGraphView
            };
        }

        public void SaveGraph(string fileName)
        {
            if (!edges.Any())
            {
                return;
            }

            DialogueContainer dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
            Edge[] connectedPorts = edges.Where(edge => edge.input.node != null).ToArray();

            for (int i = 0; i < connectedPorts.Length; i++)
            {
                DialogueNode inputNode = connectedPorts[i].input.node as DialogueNode;
                DialogueNode outputNode = connectedPorts[i].output.node as DialogueNode;

                dialogueContainer.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID,
                    PortName = connectedPorts[i].output.portName,
                    TargetNodeGUID = inputNode.GUID
                });
            }

            foreach (DialogueNode dialogueNode in nodes.Where(node => !node.EntryPoint))
            {
                dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
                {
                    NodeGUID = dialogueNode.GUID,
                    DialogueText = dialogueNode.DialogueText,
                    Position = dialogueNode.GetPosition().position
                });
            }

            if (!AssetDatabase.IsValidFolder("Assets/Grigor/Resources/DialogueGraphs"))
            {
                AssetDatabase.CreateFolder("Assets/Grigor/Resources", "DialogueGraphs");
            }

            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Grigor/Resources/DialogueGraphs/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            dialogueContainer = AssetDatabase.LoadAssetAtPath<DialogueContainer>($"Assets/Grigor/Resources/DialogueGraphs/{fileName}.asset");

            if (dialogueContainer == null)
            {
                EditorUtility.DisplayDialog( "File Not Found", "Target dialogue graph file does not exist!", "OK");

                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }

        private void ClearGraph()
        {
            nodes.Find(node => node.EntryPoint).GUID = dialogueContainer.NodeLinks[0].BaseNodeGUID;

            foreach (DialogueNode node in nodes)
            {
                if (node.EntryPoint)
                {
                    continue;
                }

                // edges.Where(edge => edge.input.node == node).ToList().ForEach(edge => graphView.RemoveElement(edge));
                edges.ForEach(edge => graphView.RemoveElement(edge));

                graphView.RemoveElement(node);
            }
        }

        private void CreateNodes()
        {
            foreach (DialogueNodeData nodeData in dialogueContainer.DialogueNodeData)
            {
                DialogueNode tempNode = graphView.CreateDialogueNode(nodeData.DialogueText);

                tempNode.GUID = nodeData.NodeGUID;
                graphView.AddElement(tempNode);

                List<NodeLinkData> nodePorts = dialogueContainer.NodeLinks.Where(data => data.BaseNodeGUID == nodeData.NodeGUID).ToList();

                nodePorts.ForEach(port => graphView.AddChoicePort(tempNode, port.PortName));
            }
        }

        private void ConnectNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                List<NodeLinkData> connections = dialogueContainer.NodeLinks.Where(data => data.BaseNodeGUID == nodes[i].GUID).ToList();

                for (int j = 0; j < connections.Count; j++)
                {
                    string targetNodeGuid = connections[j].TargetNodeGUID;
                    DialogueNode targetNode = nodes.First(node => node.GUID == targetNodeGuid);
                    LinkNodesTogether(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(dialogueContainer.DialogueNodeData.First(node => node.NodeGUID == targetNodeGuid).Position, graphView.DefaultNodeSize));
                }
            }
        }

        private void LinkNodesTogether(Port input, Port output)
        {
            Edge tempEdge = new Edge
            {
                output = output,
                input = input
            };

            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);

            graphView.Add(tempEdge);
        }
    }
}
