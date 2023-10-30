using System.Linq;
using CardboardCore.Utilities;
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
        private DialogueContainer dialogueContainer;

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
            GenerateBlackboard();
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
            rootVisualElement.Add(graphView);
        }

        private void GenerateToolbar()
        {
            Toolbar toolbar = new Toolbar();
            TextField fileNameTextField = new TextField("File Name:");

            fileNameTextField.SetValueWithoutNotify(fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(@event => fileName = @event.newValue);

            toolbar.Add(fileNameTextField);
            toolbar.Add(new Button(() => RequestDataOperation(true)) {text = "Save Data"});
            toolbar.Add(new Button(() => RequestDataOperation(false)) {text = "Load Data"});

            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(graphView);

                if (save)
                {
                    saveUtility.SaveGraph(fileName);
                }
                else
                {
                    saveUtility.LoadNarrative(fileName);
                }
            }
            else
            {
                //EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
                throw Log.Exception("Invalid file name! Please enter a valid file name.");
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
    }
}
