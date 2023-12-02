using System;
using System.Collections.Generic;
using System.Linq;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Utils.StoryGraph.Editor.Nodes;
using Grigor.Utils.StoryGraph.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Search;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Grigor.Utils.StoryGraph.Editor.Graph
{
    public class StoryGraphView : GraphView, IEdgeConnectorListener
    {
        private NodeSearchWindow searchWindow;

        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        public DialogueNode EntryPointNode;
        public Blackboard Blackboard = new Blackboard();

        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();

        public event Action RequestNodeCreationEvent;

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

        public void CreateNewDialogueNode(Vector2 position)
        {
            AddElement(CreateNode(position));
        }

        public DialogueNode CreateNode(Vector2 position, DialogueNodeData savedNodeData = null)
        {
            DialogueNodeData dialogueNodeData;
            bool loadingData = false;

            if (savedNodeData == null)
            {
                dialogueNodeData = new DialogueNodeData();

                dialogueNodeData.SetGuid(Guid.NewGuid().ToString());
            }
            else
            {
                dialogueNodeData = new DialogueNodeData(savedNodeData);
                loadingData = true;
            }

            DialogueNode dialogueNode = new DialogueNode
            {
                title = dialogueNodeData.NodeName,
                Data = dialogueNodeData,
            };

            CreateNodeTypeField(dialogueNode);
            UpdatePortsBasedOnNodeType(dialogueNode, dialogueNode.Data.NodeType, loadingData);

            if (loadingData)
            {
                CreateOverridenPorts(dialogueNode);
            }

            CreateChoiceButton(dialogueNode);
            CreateSpeakerField(dialogueNode);
            CreateDialogueTextField(dialogueNode);

            dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
            dialogueNode.SetPosition(new Rect(position, DefaultNodeSize));

            return dialogueNode;
        }

        private void CreateNodeTypeField(DialogueNode dialogueNode)
        {
            EnumField nodeTypeField = new EnumField(dialogueNode.Data.NodeType)
            {
                value = dialogueNode.Data.NodeType,
            };

            nodeTypeField.RegisterValueChangedCallback(@event =>
            {
                NodeType previousNodeType = dialogueNode.Data.NodeType;

                dialogueNode.Data.SetNodeType((NodeType)@event.newValue);

                UpdatePortsBasedOnNodeType(dialogueNode, previousNodeType);
            });

            dialogueNode.titleContainer.Add(nodeTypeField);
        }

        private bool CreateOverridenPorts(DialogueNode dialogueNode)
        {
            dialogueNode.inputContainer.Clear();
            dialogueNode.outputContainer.Clear();

            if (dialogueNode.Data.NodeType != NodeType.START)
            {
                CreateInputPort(dialogueNode);
            }

            if (dialogueNode.Data.NodeType == NodeType.END)
            {
                dialogueNode.Data.RemoveAllChoices();
            }

            foreach (DialogueChoiceData choice in dialogueNode.Data.Choices)
            {
                AddChoicePort(dialogueNode, choice);
            }

            return true;
        }

        private void UpdatePortsBasedOnNodeType(DialogueNode dialogueNode, NodeType previousNodeType, bool loadingData = false)
        {
            if (loadingData)
            {
                return;
            }

            switch (dialogueNode.Data.NodeType)
            {
                case NodeType.START:
                    if (previousNodeType is not NodeType.NONE)
                    {
                        AddChoicePort(dialogueNode);
                    }

                    dialogueNode.inputContainer.Clear();
                    break;

                case NodeType.END:
                    if (previousNodeType is not NodeType.NONE)
                    {
                        CreateInputPort(dialogueNode);
                    }

                    dialogueNode.outputContainer.Clear();
                    break;

                case NodeType.NONE:
                    if (previousNodeType is NodeType.START)
                    {
                        CreateInputPort(dialogueNode);
                    }
                    else if (previousNodeType is NodeType.END)
                    {
                        AddChoicePort(dialogueNode);
                    }
                    else
                    {
                        CreateInputPort(dialogueNode);
                        AddChoicePort(dialogueNode);
                    }
                    break;
            }
        }

        private Port CreateInputPort(DialogueNode dialogueNode)
        {
            dialogueNode.inputContainer.Clear();

            Port inputPort = GetPortInstance(dialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "";

            dialogueNode.inputContainer.Add(inputPort);

            return inputPort;
        }

        private TextField CreateDialogueTextField(DialogueNode dialogueNode)
        {
            TextField dialogueTextField = new TextField("")
            {
                multiline = true,
                style =
                {
                    whiteSpace = WhiteSpace.Normal
                }
            };

            dialogueTextField.RegisterValueChangedCallback(@event =>
            {
                dialogueNode.Data.SetDialogueText(@event.newValue);
                dialogueNode.Data.SetNodeName(@event.newValue);
            });

            dialogueTextField.SetValueWithoutNotify(dialogueNode.title);

            dialogueNode.mainContainer.Add(dialogueTextField);

            return dialogueTextField;
        }

        private Button CreateChoiceButton(DialogueNode dialogueNode)
        {
            Button createChoicePortButton = new Button(() => { AddChoicePort(dialogueNode); })
            {
                text = "Add Choice"
            };

            dialogueNode.titleButtonContainer.Add(createChoicePortButton);

            return createChoicePortButton;
        }

        private ObjectField CreateSpeakerField(DialogueNode dialogueNode)
        {
            ObjectField speakerField = new ObjectField("Speaker")
            {
                value = dialogueNode.Data.Speaker,
                objectType = typeof(CharacterData)
            };

            speakerField.RegisterValueChangedCallback(@event => {
                dialogueNode.Data.SetSpeaker(@event.newValue as CharacterData);
            });

            dialogueNode.mainContainer.Add(speakerField);

            return speakerField;
        }

        private void AddChoicePort(DialogueNode nodeCache, DialogueChoiceData existingChoice = null)
        {
            DialogueChoiceData choice = new();

            bool choiceOverriden = existingChoice != null;

            if (choiceOverriden)
            {
                choice = existingChoice;
            }

            Port generatedPort = GetPortInstance(nodeCache, Direction.Output);

            generatedPort.AddManipulator(new EdgeConnector<Edge>(this));

            Label portLabel = generatedPort.contentContainer.Q<Label>("type");

            generatedPort.contentContainer.Remove(portLabel);

            VisualElement connector = generatedPort.contentContainer.Q<VisualElement>("connector");
            connector.pickingMode = PickingMode.Position;

            int outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count;
            string outputPortName = choiceOverriden ? existingChoice.Text : $"{outputPortCount + 1}";

            generatedPort.portName = outputPortName;

            if (!choiceOverriden)
            {
                choice.SetChoiceText(generatedPort.portName);

                nodeCache.Data.AddChoice(choice);
            }

            TextField textField = new TextField
            {
                name = string.Empty,
                value = outputPortName
            };

            textField.RegisterValueChangedCallback(@event =>
            {
                choice.SetChoiceText(@event.newValue);
                generatedPort.portName = @event.newValue;
            });

            textField.AddToClassList("DialogueChoiceTextField");

            textField.pickingMode = PickingMode.Ignore;
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

        private void RemovePort(DialogueNode dialogueNode, Port socket)
        {
            dialogueNode.Data.RemoveChoiceByChoiceText(socket.portName);

            IEnumerable<Edge> targetEdges = edges.ToList().Where(edge => edge.output.portName == socket.portName && edge.output.node == socket.node);

            if (targetEdges.Any())
            {
                Edge edge = targetEdges.First();
                edge.input.Disconnect(edge);
                RemoveElement(edge);
            }

            dialogueNode.outputContainer.Remove(socket);
            dialogueNode.RefreshPorts();
            dialogueNode.RefreshExpandedState();
        }

        private Port GetPortInstance(DialogueNode node, Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            RequestNodeCreationEvent?.Invoke();
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {

        }
    }
}
