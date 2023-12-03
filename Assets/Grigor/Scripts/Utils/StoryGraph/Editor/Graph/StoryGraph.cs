using System;
using System.Linq;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Utils.StoryGraph.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grigor.Utils.StoryGraph.Editor.Graph
{
    public class StoryGraph : EditorWindow
    {
        private string fileName = "New Narrative";

        private StoryGraphView graphView;
        private DialogueGraphData dialogueGraphData;
        private CharacterData defaultSpeaker;

        public CharacterData DefaultSpeaker => defaultSpeaker;

        [MenuItem("Grigor/Narrative Graph")]
        public static void CreateGraphViewWindow()
        {
            StoryGraph window = GetWindow<StoryGraph>();
            window.titleContent = new GUIContent("Narrative Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
            // GenerateBlackboard();

            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        private void ConstructGraphView()
        {
            graphView = new StoryGraphView(this)
            {
                name = "Narrative Graph",
            };

            graphView.StretchToParentSize();
            graphView.RequestNodeCreationEvent += OnRequestNodeCreation;

            rootVisualElement.Add(graphView);
        }

        private void GenerateToolbar()
        {
            Toolbar toolbar = new Toolbar();

            TextField fileNameTextField = new TextField("");

            fileNameTextField.SetValueWithoutNotify(fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(@event => fileName = @event.newValue);

            toolbar.Add(new Button(() => RequestDataOperation(DataOperationType.Save)) {text = "Save Data"});
            toolbar.Add(new Button(() => RequestDataOperation(DataOperationType.Load)) {text = "Load Data"});

            ObjectField speaker = new ObjectField() { objectType = typeof(CharacterData), value = defaultSpeaker };
            speaker.RegisterValueChangedCallback(@event => defaultSpeaker = @event.newValue as CharacterData);

            ObjectField dialogueAsset = new ObjectField() { objectType = typeof(DialogueGraphData), value = dialogueGraphData };
            dialogueAsset.RegisterValueChangedCallback(@event =>
            {
                dialogueGraphData = @event.newValue as DialogueGraphData;

                if (dialogueGraphData != null)
                {
                    RequestDataOperation(DataOperationType.Load);
                }
            });

            toolbar.Add(dialogueAsset);
            toolbar.Add(speaker);

            toolbar.Add(new Button(() =>
            {
                if (dialogueGraphData != null)
                {
                    RequestDataOperation(DataOperationType.Save);
                }

                RequestDataOperation(DataOperationType.Create);

                dialogueAsset.value = dialogueGraphData;
            })
            {
                text = "Create New"
            });

            toolbar.Add(fileNameTextField);

            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(DataOperationType dataOperationType)
        {
            GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(graphView);

            switch (dataOperationType)
            {
                case DataOperationType.Create:
                    DialogueGraphData newDialogueGraphData = saveUtility.CreateNewGraphAsset(fileName);
                    dialogueGraphData = newDialogueGraphData;
                    break;

                case DataOperationType.Save:
                    saveUtility.SaveGraph(dialogueGraphData);
                    break;

                case DataOperationType.Load:
                    saveUtility.LoadGraph(dialogueGraphData);
                    break;

                default:
                    throw Log.Exception($"Data operation {dataOperationType} is not supported!");
            }
        }

        private void GenerateMiniMap()
        {
            MiniMap miniMap = new MiniMap
            {
                anchored = true
            };

            Vector2 coordinates = graphView.contentViewContainer.WorldToLocal(new Vector2(maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(coordinates.x, coordinates.y, 200, 140));

            graphView.Add(miniMap);
        }

        private void GenerateBlackboard()
        {
            Blackboard blackboard = new Blackboard(graphView);

            blackboard.Add(new BlackboardSection {title = "Exposed Variables"});

            blackboard.addItemRequested = blackboard =>
            {
                graphView.AddPropertyToBlackboard(ExposedProperty.CreateInstance(), false);
            };

            blackboard.editTextRequested = (blackboard, element, newValue) => {

                BlackboardField property = (BlackboardField)element;

                string oldPropertyName = property.text;

                if (graphView.ExposedProperties.Any(exposedProperty => exposedProperty.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exists, please chose another one.", "OK");
                    return;
                }

                int targetIndex = graphView.ExposedProperties.FindIndex(exposedProperty => exposedProperty.PropertyName == oldPropertyName);

                graphView.ExposedProperties[targetIndex].PropertyName = newValue;
                property.text = newValue;
            };

            blackboard.SetPosition(new Rect(10, 30, 200, 300));

            graphView.Add(blackboard);
            graphView.Blackboard = blackboard;
        }

        private void OnKeyDown(KeyDownEvent @event)
        {
            Vector2 graphMousePosition = Event.current.mousePosition;

            switch (@event.keyCode)
            {
                case KeyCode.C:
                {
                    Rect rect = new Rect(graphMousePosition, graphView.DefaultCommentBlockSize);
                    graphView.CreateCommentBlock(rect);
                    break;
                }
                case KeyCode.Space:
                    graphView.CreateNewDialogueNode(graphMousePosition);
                    break;

                case KeyCode.Tab:
                    graphView.CreateNewDialogueNode(graphMousePosition);
                    break;
                case KeyCode.S:
                    if (Event.current.control)
                    {
                        RequestDataOperation(DataOperationType.Save);
                    }

                    break;
            }
        }

        private void OnRequestNodeCreation()
        {
            Vector2 mousePosition = Event.current.mousePosition;

            graphView.CreateNewDialogueNode(mousePosition);
        }
    }
}
