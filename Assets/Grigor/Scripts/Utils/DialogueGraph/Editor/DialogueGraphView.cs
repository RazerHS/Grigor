using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RazerCore.Utils.DialogueGraph.Editor
{
    public class DialogueGraphView : GraphView
    {
        public Vector2 DefaultNodeSize => new Vector2(150, 200);

        public DialogueGraphView()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Grigor/Resources/DialogueGraph.uss"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryPointNode());
        }

        private DialogueNode GenerateEntryPointNode()
        {
            DialogueNode node = new DialogueNode
            {
                title = "START",
                GUID = System.Guid.NewGuid().ToString(),
                DialogueText = "Banana",
                EntryPoint = true
            };

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            Port generatedPort = GeneratePort(node, Direction.Output);
            generatedPort.portName = "Next";
            node.outputContainer.Add(generatedPort);

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }

        private Port GeneratePort(DialogueNode targetNode, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return targetNode.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        public void CreateNode(string nodeName)
        {
            AddElement(CreateDialogueNode(nodeName));
        }

        public DialogueNode CreateDialogueNode(string nodeName)
        {
            DialogueNode dialogueNode = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = System.Guid.NewGuid().ToString()
            };

            Port inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            dialogueNode.inputContainer.Add(inputPort);

            Button button = new Button(() => { AddChoicePort(dialogueNode); });
            button.text = "New Choice";
            dialogueNode.titleContainer.Add(button);

            TextField dialogueTextField = new TextField(string.Empty);

            dialogueTextField.RegisterValueChangedCallback(changeEvent =>
            {
                dialogueNode.DialogueText = changeEvent.newValue;
                dialogueNode.title = changeEvent.newValue;
            });

            dialogueTextField.SetValueWithoutNotify(dialogueNode.title);
            dialogueNode.mainContainer.Add(dialogueTextField);

            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();

            dialogueNode.SetPosition(new Rect(Vector2.zero, DefaultNodeSize));

            return dialogueNode;
        }

        public void AddChoicePort(DialogueNode dialogueNode, string overridenPortName = "")
        {
            Port generatedPort = GeneratePort(dialogueNode, Direction.Output);

            Label oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);

            int outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
            generatedPort.portName = $"Choice {outputPortCount}";

            string choicePortName = string.IsNullOrEmpty(overridenPortName) ? $"Choice {outputPortCount + 1}" : overridenPortName;

            TextField choiceTextField = new TextField
            {
                name = string.Empty,
                value = choicePortName,
            };

            choiceTextField.RegisterValueChangedCallback(changeEvent => generatedPort.portName = changeEvent.newValue);

            generatedPort.contentContainer.Add(new Label(" "));
            generatedPort.contentContainer.Add(choiceTextField);

            Button deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
            {
                text = "X"
            };

            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = choicePortName;

            dialogueNode.outputContainer.Add(generatedPort);
            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
        }

        private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
        {
            List<Edge> targetEdges = edges.ToList().Where(edge => edge.output.portName == generatedPort.portName && edge.output.node == generatedPort.node).ToList();

            if (targetEdges.Any())
            {
                Edge edge = targetEdges.First();

                edge.input.Disconnect(edge);
                RemoveElement(edge);
            }

            dialogueNode.outputContainer.Remove(generatedPort);
            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }
    }
}
