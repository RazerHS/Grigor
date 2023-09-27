using CardboardCore.Utilities;
using UnityEditor;
using UnityEngine;

namespace Grigor.Utils
{
    public static class AssetLoader
    {
        public static T LoadAsset<T>(string assetName, T asset) where T : ScriptableObject
        {
            if (asset != null)
            {
                return asset;
            }

            string path = $"Assets/Resources/{assetName}.asset";

            T newAsset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (newAsset == null)
            {
                throw Log.Exception($"{assetName} cannot be found at path {path}!");
            }

            return newAsset;
        }
    }
}
