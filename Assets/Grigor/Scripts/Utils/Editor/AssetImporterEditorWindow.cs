using System.Collections.Generic;
using System.IO;
using System.Linq;
using CardboardCore.Utilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace RazerCore.Utils.Editor
{
    public class AssetImporterEditorWindow : OdinEditorWindow
    {
        [SerializeField] private List<Texture2D> textures;
        [SerializeField] private List<Material> materials;
        [SerializeField] private Shader shader;
        [SerializeField, ValueDropdown(nameof(GetShaderPropertyList))] private string property;
        [SerializeField, ValidateInput(nameof(OnValidateModel))] private Object model;

        [Button("Revalidate Model", ButtonSizes.Large)]
        public void RevalidateModel()
        {
            OnValidateModel(model);
        }

        [MenuItem("Grigor/Asset Importer")]
        protected static void OpenWindow()
        {
            GetWindow<AssetImporterEditorWindow>().Show();
        }

        private bool OnValidateFolder(Object value)
        {
            string filePath = AssetDatabase.GetAssetPath(value);

            return AssetDatabase.IsValidFolder(filePath);
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

            return true;
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
