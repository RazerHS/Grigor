using System;
using System.Collections.Generic;
using CardboardCore.Utilities;
using MEC;
using UnityEditor;
using UnityEngine;

namespace Grigor.Utils
{
    public static class Helper
    {
        private static IEnumerator<float> DelayCoroutine(float delay, Action callback)
        {
            yield return Timing.WaitForSeconds(delay);

            callback?.Invoke();
        }

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

        public static void Delay(float delay, Action callback)
        {
            Timing.RunCoroutine(DelayCoroutine(delay, callback));
        }
    }
}
