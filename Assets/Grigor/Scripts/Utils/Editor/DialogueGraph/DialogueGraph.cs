using System;
using System.Resources;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RazerCore.Utils.Editor.DialogueGraph
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

            toolbar.Add(new Button(SaveData) { text = "Save Data" });
            toolbar.Add(new Button(LoadData) { text = "Load Data" });

            Button createNodeButton = new Button(() => {
                graphView.CreateNode("Dialogue Node");
            });

            createNodeButton.text = "Create Node";

            toolbar.Add(createNodeButton);

            rootVisualElement.Add(toolbar);
        }

        private void LoadData()
        {
            throw new NotImplementedException();
        }

        private void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}
