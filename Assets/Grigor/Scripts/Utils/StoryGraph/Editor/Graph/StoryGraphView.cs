using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Grigor.Utils.StoryGraph.Editor.Nodes;
using Grigor.Utils.StoryGraph.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Grigor.Utils.StoryGraph.Editor.Graph
{
    public class StoryGraphView : GraphView
    {
        private NodeSearchWindow searchWindow;

        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        public DialogueNode EntryPointNode;
        public Blackboard Blackboard = new Blackboard();

        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();

        public StoryGraphView(StoryGraph editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("StoryGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            GridBackground grid = new GridBackground();

            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GetEntryPointNodeInstance());
            AddSearchWindow(editorWindow);
        }

        private void AddSearchWindow(StoryGraph editorWindow)
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(editorWindow, this);

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        public void ClearBlackboardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
        {
            commentBlockData ??= new CommentBlockData();

            Group group = new Group
            {
                autoUpdateGeometry = true,
                title = commentBlockData.Title
            };

            AddElement(group);
            group.SetPosition(rect);

            return group;
        }

        public void AddPropertyToBlackboard(ExposedProperty property, bool loadMode = false)
        {
            string localPropertyName = property.PropertyName;
            string localPropertyValue = property.PropertyValue;

            if (!loadMode)
            {
                while (ExposedProperties.Any(exposedProperty => exposedProperty.PropertyName == localPropertyName))
                {
                    localPropertyName = $"{localPropertyName}(1)";
                }
            }

            ExposedProperty newProperty = ExposedProperty.CreateInstance();

            newProperty.PropertyName = localPropertyName;
            newProperty.PropertyValue = localPropertyValue;

            ExposedProperties.Add(newProperty);

            VisualElement container = new VisualElement();

            BlackboardField field = new BlackboardField
            {
                text = localPropertyName, typeText = "string"
            };

            container.Add(field);

            var propertyValueTextField = new TextField("Value:")
            {
                value = localPropertyValue
            };

            propertyValueTextField.RegisterValueChangedCallback(@event =>
            {
                var index = ExposedProperties.FindIndex(exposedProperty => exposedProperty.PropertyName == newProperty.PropertyName);
                ExposedProperties[index].PropertyValue = @event.newValue;
            });

            BlackboardRow blackboardRow = new BlackboardRow(field, propertyValueTextField);

            container.Add(blackboardRow);
            Blackboard.Add(container);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            Port startPortView = startPort;

            ports.ForEach(port =>
            {
                if (startPortView != port && startPortView.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public void CreateNewDialogueNode(string nodeName, Vector2 position)
        {
            AddElement(CreateNode(nodeName, position));
        }

        public DialogueNode CreateNode(string nodeName, Vector2 position)
        {
            DialogueNode dialogueNode = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };

            dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            Port inputPort = GetPortInstance(dialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";

            dialogueNode.inputContainer.Add(inputPort);
            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();

            // TO-DO: implement screen center instantiation positioning
            dialogueNode.SetPosition(new Rect(position, DefaultNodeSize));

            TextField textField = new TextField("");

            textField.RegisterValueChangedCallback(@event =>
            {
                dialogueNode.DialogueText = @event.newValue;
                dialogueNode.title = @event.newValue;
            });

            textField.SetValueWithoutNotify(dialogueNode.title);
            dialogueNode.mainContainer.Add(textField);

            Button button = new Button(() => { AddChoicePort(dialogueNode); })
            {
                text = "Add Choice"
            };

            dialogueNode.titleButtonContainer.Add(button);

            return dialogueNode;
        }

        public void AddChoicePort(DialogueNode nodeCache, string overriddenPortName = "")
        {
            Port generatedPort = GetPortInstance(nodeCache, Direction.Output);
            Label portLabel = generatedPort.contentContainer.Q<Label>("type");

            generatedPort.contentContainer.Remove(portLabel);

            VisualElement connector = generatedPort.contentContainer.Q<VisualElement>("connector");
            connector.pickingMode = PickingMode.Position;

            int outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count;
            string outputPortName = string.IsNullOrEmpty(overriddenPortName) ? $"Option {outputPortCount + 1}" : overriddenPortName;

            generatedPort.portName = outputPortName;

            TextField textField = new TextField
            {
                name = string.Empty,
                value = outputPortName
            };

            textField.RegisterValueChangedCallback(@event => generatedPort.portName = @event.newValue);
            textField.AddToClassList("DialogueChoiceTextField");

            textField.style.minWidth = 60;
            textField.style.maxWidth = 100;

            generatedPort.contentContainer.Add(new Label(" "));
            generatedPort.contentContainer.Add(textField);

            Button deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
            {
                text = "X"
            };

            generatedPort.contentContainer.Add(deleteButton);

            nodeCache.outputContainer.Add(generatedPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

        private void RemovePort(Node node, Port socket)
        {
            IEnumerable<Edge> targetEdges = edges.ToList().Where(x => x.output.portName == socket.portName && x.output.node == socket.node);

            if (targetEdges.Any())
            {
                Edge edge = targetEdges.First();

                edge.input.Disconnect(edge);
                RemoveElement(targetEdges.First());
            }

            node.outputContainer.Remove(socket);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GetPortInstance(DialogueNode node, Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        private DialogueNode GetEntryPointNodeInstance()
        {
            var nodeCache = new DialogueNode
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "ENTRYPOINT",
                EntryPoint = true
            };

            Port generatedPort = GetPortInstance(nodeCache, Direction.Output);
            generatedPort.portName = "Next";

            nodeCache.outputContainer.Add(generatedPort);

            nodeCache.capabilities &= ~Capabilities.Movable;
            nodeCache.capabilities &= ~Capabilities.Deletable;

            nodeCache.RefreshExpandedState();
            nodeCache.RefreshPorts();
            nodeCache.SetPosition(new Rect(100, 200, 100, 150));

            return nodeCache;
        }
    }
}
