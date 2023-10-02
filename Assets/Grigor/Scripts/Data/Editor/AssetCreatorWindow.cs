﻿using System;
using CardboardCore.Utilities;
using Grigor.Utils;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Grigor.Data.Editor
{
    public abstract class AssetCreatorWindow<T> : OdinMenuEditorWindow where T : ScriptableObjectData
    {
        protected virtual string dataName => "Data";
        protected virtual string dataAssetsPath => "Assets/Grigor/Data";

        protected DataStorage dataStorage;

        private NewData newData;
        protected T currentInstance;
        protected OdinMenuTreeSelection selection;

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree
            {
                DefaultMenuStyle =
                {
                    IconSize = 20f,
                    BorderPadding = 15f,
                    AlignTriangleLeft = true,
                    TriangleSize = 9.64f,
                    TrianglePadding = -4.06f,
                    Offset = 20f,
                    Height = 24,
                    IconPadding = 2f,
                    IconOffset = 0f,
                    BorderAlpha = 1f,
                    SelectedColorDarkSkin = new Color(0.877f, 0.095f, 0.454f, 0.192f),
                }
            };

            newData = new NewData(dataAssetsPath);
            newData.NewDataCreatedEvent += OnNewDataCreated;

            tree.Add($"New {dataName}", newData);
            tree.AddAllAssetsAtPath(dataName, dataAssetsPath, typeof(T), true, true);

            tree.Config.DrawSearchToolbar = true;

            OnDrawTree(tree);

            return tree;
        }

        protected virtual void OnDrawTree(OdinMenuTree tree) { }

        protected override void OnBeginDrawEditors()
        {
            if (MenuTree.Selection != null)
            {
                selection = MenuTree.Selection;
            }

            SirenixEditorGUI.BeginHorizontalToolbar();
            GUILayout.FlexibleSpace();

            RefreshAllAssetsButton();
            DeleteButton();

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private void DeleteButton()
        {
            if (SirenixEditorGUI.ToolbarButton($"Delete Current {dataName}"))
            {
                DeleteAsset();
            }
        }

        private void DeleteAsset()
        {
            T asset = selection.SelectedValue as T;
            string dataAssetPath = AssetDatabase.GetAssetPath(asset);

            if (asset == null)
            {
                return;
            }

            AssetDatabase.DeleteAsset(dataAssetPath);
            AssetDatabase.SaveAssets();

            dataStorage.UpdateData();
        }

        private void RefreshAllAssetsButton()
        {
            if (SirenixEditorGUI.ToolbarButton("Refresh All Data"))
            {
                dataStorage.UpdateData();
            }
        }

        protected override void OnDestroy()
        {
            if (newData == null)
            {
                return;
            }

            newData.NewDataCreatedEvent -= OnNewDataCreated;

            DestroyImmediate(newData.Data);

            base.OnDestroy();
        }

        private void OnNewDataCreated(T data)
        {
            currentInstance = data;

            OnDataUpdated(data);
        }

        private void OnDataUpdated(T instance)
        {
            if (dataStorage == null)
            {
                dataStorage = Helper.LoadAsset("DataStorage", dataStorage);
            }

            dataStorage.UpdateData();

            EditorUtility.SetDirty(dataStorage);
            AssetDatabase.SaveAssets();
        }

        protected void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }

        public class NewData
        {
            [SerializeField, InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)] private T data;

            private readonly string dataAssetsPath;

            public T Data => data;

            public event Action<T> NewDataCreatedEvent;

            public NewData(string dataAssetsPath)
            {
                this.dataAssetsPath = dataAssetsPath;

                data = CreateInstance<T>();
                data.name = "New Data";
            }

            [Button("Create New", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
            private void CreateNewData()
            {
                if (data.AssetName == null)
                {
                    throw Log.Exception("Cannot create data without a name!");
                }

                string pathToData = $"{dataAssetsPath}/{data.AssetName}.asset";

                AssetDatabase.CreateAsset(data, pathToData);
                AssetDatabase.SaveAssets();

                data = CreateInstance<T>();

                NewDataCreatedEvent?.Invoke(data);
            }
        }
    }
}