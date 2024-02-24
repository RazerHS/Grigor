#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CardboardCore.Utilities;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace RazerCore.Utils.AssetImporter.Editor
{
    public class AssetImporterEditorWindow : OdinEditorWindow
    {
        [SerializeField, HorizontalGroup("Selection", Width = 0.5f), ColoredBoxGroup("Selection/Selection", false, true), LabelWidth(60), InlineButton("@shader = null", SdfIconType.BootstrapReboot, "")] private Shader shader;
        [SerializeField, HorizontalGroup("Selection"), ColoredBoxGroup("Selection/Selection"), LabelWidth(60), InlineButton(nameof(ResetModel), SdfIconType.BootstrapReboot, "")] private Object model;

        [SerializeField, HorizontalGroup("Selection/Right"), ColoredBoxGroup("Selection/Right/Config", false, true), LabelWidth(220)] private bool automaticallyFindTexturesFolder = true;
        [SerializeField, HorizontalGroup("Selection/Right"), ColoredBoxGroup("Selection/Right/Config", false, true), LabelWidth(220)] private bool showDebug;
        [SerializeField, HorizontalGroup("Selection/Right"), ColoredBoxGroup("Selection/Right/Config", false, true), LabelWidth(220)] private bool resetBaseColor;
        // [SerializeField, HorizontalGroup("Selection/Right"), ColoredBoxGroup("Selection/Right/Config", false, true), LabelWidth(220)] private bool generatePrefab;
        [SerializeField, HorizontalGroup("Selection"), ColoredBoxGroup("Selection/Selection"), HideIf(nameof(automaticallyFindTexturesFolder))] private Object texturesFolder;

        [SerializeField, HorizontalGroup("Debug", Width = 0.5f), ListDrawerSettings(DraggableItems = false, ShowFoldout = false), ShowIf(nameof(showDebug)), ReadOnly] private List<Texture2D> textures;
        [SerializeField, HorizontalGroup("Debug"), ListDrawerSettings(DraggableItems = false, ShowFoldout = false), ShowIf(nameof(showDebug)), ReadOnly] private List<Material> materials;

        [SerializeField, HorizontalGroup("Selectors", Width = 0.3f), ListDrawerSettings(DraggableItems = false, ShowItemCount = false, ShowFoldout = false, HideAddButton = true, HideRemoveButton = true), LabelText("Textures from Shader"), HideIf("@shader == null")] private List<ShaderPropertySelector> propertySelector = new();
        [SerializeField, HorizontalGroup("Selectors/Right"), ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true), HideIf("@model == null")] private List<ModelAssetData> modelAssetData;

        [Button("Extract Materials and Textures", ButtonSizes.Large), ColoredBoxGroup("Selection/Selection"), DisableIf("@model == null || shader == null"), VerticalGroup("Selection/Selection/vert", PaddingTop = 5)] private void Extract() => ExtractMaterialsAndTextures();

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        [MenuItem("Grigor/Asset Importer")]
        protected static void OpenWindow()
        {
            GetWindow<AssetImporterEditorWindow>().Show();
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

        [OnInspectorDispose]
        private void OnInspectorDispose()
        {
            ResetModel();
        }

        private void ResetModel()
        {
            model = null;
            modelAssetData = null;

            materials.Clear();
            textures.Clear();
        }

        private void ExtractMaterialsAndTextures()
        {
            string modelPath = AssetDatabase.GetAssetPath(model);

            string modelDirectory = Path.GetDirectoryName(modelPath);

            if (string.IsNullOrEmpty(modelDirectory))
            {
                return;
            }

            if (!modelPath.EndsWith(".fbx"))
            {
                model = null;

                throw Log.Exception("Object is not an .fbx file!");
            }

            string texturesPath = Path.Combine(modelDirectory, "Textures");

            if (!automaticallyFindTexturesFolder)
            {
                texturesPath = AssetDatabase.GetAssetPath(texturesFolder);
            }

            textures = LoadAllAssetsAtPath<Texture2D>(texturesPath);

            materials = ExtractMaterials(modelPath);

            Refresh();
        }

        private void Refresh()
        {
            RefreshShaderProperties();
            LoadModelAssetData();
            ApplyTexturesToMaterials();
            // FindPrefabAndReplaceModel();
        }

        // private void FindPrefabAndReplaceModel()
        // {
        //     if (!generatePrefab)
        //     {
        //         return;
        //     }
        //
        //     string modelPath = AssetDatabase.GetAssetPath(model);
        //     string modelDirectory = Path.GetDirectoryName(modelPath);
        //
        //     if (string.IsNullOrEmpty(modelDirectory))
        //     {
        //         return;
        //     }
        //
        //     string prefabPath = Path.Combine(modelDirectory, $"{model.name}.prefab");
        //
        //     GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        //
        //     if (prefab == null)
        //     {
        //         return;
        //     }
        //
        //     GameObject modelGameObject = (GameObject)model;
        //
        //     GameObject newPrefab = PrefabUtility.ReplacePrefab(modelGameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
        //
        //     if (newPrefab == null)
        //     {
        //         throw Log.Exception("Failed to replace prefab!");
        //     }
        // }

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

        private void ApplyTexturesToMaterials()
        {
            for (int i = 0; i < modelAssetData.Count; i++)
            {
                Material material = modelAssetData[i].Material;

                material.shader = shader;

                if (resetBaseColor)
                {
                    material.SetColor(BaseColor, Color.white);
                }

                for (int j = 0; j < modelAssetData[i].MaterialAssets.Count; j++)
                {
                    material.SetTexture(modelAssetData[i].MaterialAssets[j].Property, modelAssetData[i].MaterialAssets[j].Texture);
                }

                EditorUtility.SetDirty(material);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
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

                if (shader.GetPropertyType(i) != ShaderPropertyType.Texture)
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

            if (result == 0 && extractedMaterials.Count != 0)
            {
                Log.Write("Materials already extracted.");
            }

            Log.Write($"Loaded <b>{extractedMaterials.Count}</b> materials from the model's directory!");

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

#endif
