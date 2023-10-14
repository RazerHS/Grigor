using System;
using System.Resources;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RazerCore.Utils.DialogueGraph.Editor
{
    public class DialogueGraph : EditorWindow
    {
        private DialogueGraphView graphView;
        private string fileName;

        [MenuItem("Grigor/Dialogue Graph")]
        public static void OpenDialogueGraphWindow()
        {
            DialogueGraph window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMinimap();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        private void ConstructGraphView()
        {
            graphView = new DialogueGraphView()
            {
                name = "Dialogue Graph"
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
            fileNameTextField.RegisterValueChangedCallback(changeEvent => fileName = changeEvent.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

            Button createNodeButton = new Button(() => {
                graphView.CreateNode("Dialogue Node");
            });

            createNodeButton.text = "Create Node";

            toolbar.Add(createNodeButton);

            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK");
                return;
            }

            GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(graphView);

            if (save)
            {
                saveUtility.SaveGraph(fileName);
            }
            else
            {
                saveUtility.LoadGraph(fileName);
            }
        }

        private void GenerateMinimap()
        {
            MiniMap miniMap = new MiniMap { anchored = true };
            miniMap.SetPosition(new Rect(10, 30, 200, 140));

            graphView.Add(miniMap);
        }
    }
}
