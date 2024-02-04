using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CardboardCore.Utilities;
using Grigor.Utils;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace RazerCore.Utils.AssetImporter.Editor
{
    public class AssetImporterEditorWindow : OdinEditorWindow
    {
        [SerializeField] private List<Texture2D> textures;
        [SerializeField] private List<Material> materials;
        [SerializeField, ListDrawerSettings(DraggableItems = false)] private List<ShaderPropertySelector> propertySelector = new();
        [SerializeField] private Shader shader;
        [SerializeField, ValueDropdown(nameof(GetShaderPropertyList))] private string property;
        [SerializeField] private Object model;

        [SerializeField] private List<ModelAssetData> modelAssetData;

        [Button("Revalidate Model", ButtonSizes.Large)]
        public void ValidateModel()
        {
            OnValidateModel(model);
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (!Helper.GetKeyPressed(out KeyCode key))
            {
                return;
            }

            if (key != KeyCode.R)
            {
                return;
            }

            Refresh();
        }

        private void Refresh()
        {
            Log.Write("refresh!");

            RefreshShaderProperties();
            LoadModelAssetData();
        }

        private void RefreshShaderProperties()
        {
            List<string> enabledProperties = propertySelector.Where(propertySelector => propertySelector.Enable).Select(propertySelector => propertySelector.Property).ToList();

            propertySelector.Clear();

            if (shader == null)
            {
                throw Log.Exception("No shader selected!");
            }

            List<string> exposedProperties = GetShaderPropertyList();

            foreach (string exposedProperty in exposedProperties)
            {
                ShaderPropertySelector shaderPropertySelector = new ShaderPropertySelector(exposedProperty);

                if (!enabledProperties.Contains(shaderPropertySelector.Property))
                {
                    shaderPropertySelector.SetEnabled(false);
                }

                propertySelector.Add(shaderPropertySelector);
            }
        }

        [MenuItem("Grigor/Asset Importer")]
        protected static void OpenWindow()
        {
            GetWindow<AssetImporterEditorWindow>().Show();
        }

        private bool OnValidateModel(Object value)
        {
            string modelPath = AssetDatabase.GetAssetPath(model);

            string modelDirectory = Path.GetDirectoryName(modelPath);

            if (string.IsNullOrEmpty(modelDirectory))
            {
                return false;
            }

            if (!modelPath.EndsWith(".fbx"))
            {
                model = null;

                throw Log.Exception("Object is not an .fbx file!");
            }

            string texturesPath = Path.Combine(modelDirectory, "Textures");

            textures = LoadAllAssetsAtPath<Texture2D>(texturesPath);

            materials = ExtractMaterials(modelPath);

            LoadModelAssetData();

            return true;
        }

        private void LoadModelAssetData()
        {
            modelAssetData.Clear();

            for (int i = 0; i < materials.Count; i++)
            {
                modelAssetData.Add(new ModelAssetData());

                modelAssetData[i].SetMaterial(materials[i]);

                modelAssetData[i].SetMaterialAssets(LoadMaterialAssets(modelAssetData[i]));
            }
        }

        private List<MaterialAssets> LoadMaterialAssets(ModelAssetData modelAssetData)
        {
            List<MaterialAssets> materialAssets = new();

            List<string> enabledProperties = propertySelector.Where(shaderProperty => shaderProperty.Enable).Select(shaderProperty => shaderProperty.Property).ToList();

            for (int i = 0; i < enabledProperties.Count; i++)
            {
                MaterialAssets newMaterialAssets = new();

                string property = enabledProperties[i];

                newMaterialAssets.SetProperty(property);
                newMaterialAssets.SetTexture(FindCorrectTexture(property, modelAssetData.Material.name));

                materialAssets.Add(newMaterialAssets);
            }

            return materialAssets;
        }

        private Texture2D FindCorrectTexture(string property, string materialName)
        {
            foreach (Texture2D texture in textures)
            {
                if (!texture.name.Contains(property))
                {
                    continue;
                }

                if (!texture.name.Contains(materialName))
                {
                    continue;
                }

                string newTextureName = $"Tex_{model.name[(model.name.IndexOf("_", StringComparison.Ordinal) + 1)..]}_{materialName}{property}";

                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(texture), newTextureName);

                return texture;
            }

            Log.Error($"No texture found for property <b>{property}</b> and material: <b>{materialName}</b>!");

            return null;
        }

        private List<string> GetShaderPropertyList()
        {
            List<string> properties = new();

            if (shader == null)
            {
                return properties;
            }

            int propertyCount = shader.GetPropertyCount();

            for (int i = 0; i < propertyCount; i++)
            {
                if ((shader.GetPropertyFlags(i) & ShaderPropertyFlags.HideInInspector) != 0)
                {
                    continue;
                }

                properties.Add(shader.GetPropertyName(i));
            }

            return properties;
        }

        private List<Material> ExtractMaterials(string modelPath)
        {
            int result = 0;

            try
            {
                AssetDatabase.StartAssetEditing();

                HashSet<string> assetsToReload = new HashSet<string>();

                IEnumerable<Object> materialsToExtract = AssetDatabase.LoadAllAssetsAtPath(modelPath).Where(x => x.GetType() == typeof(Material));

                foreach (Object material in materialsToExtract)
                {
                    result++;

                    string newAssetPath = modelPath[..(modelPath.Length - model.name.Length - 4)] + material.name + ".mat";

                    newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);
                    string error = AssetDatabase.ExtractAsset(material, newAssetPath);

                    if (string.IsNullOrEmpty(error))
                    {
                        assetsToReload.Add(modelPath);
                    }
                }

                foreach (string path in assetsToReload)
                {
                    AssetDatabase.WriteImportSettingsIfDirty(path);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            List<Material> extractedMaterials = LoadAllAssetsAtPath<Material>(Path.GetDirectoryName(modelPath));

            if (result != extractedMaterials.Count)
            {
                throw Log.Exception("Other materials from the model directory were also loaded!");
            }

            return extractedMaterials;
        }

        private List<T> LoadAllAssetsAtPath<T>(string path) where T : Object
        {
            List<T> assetsToLoad = new();

            if (!Directory.Exists(path))
            {
                return assetsToLoad;
            }

            string[] assets = Directory.GetFiles(path);

            foreach (string assetPath in assets)
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset == null)
                {
                    continue;
                }

                assetsToLoad.Add(asset);
            }

            return assetsToLoad;
        }
    }
}
