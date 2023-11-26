using System;
using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Data.Editor;
using JetBrains.Annotations;
using MEC;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEngine;

namespace Grigor.Utils
{
    public static class Helper
    {
        private static KeyCode lastKeyPressed;

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

            string path = $"Assets/Grigor/Resources/{assetName}.asset";

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

#if UNITY_EDITOR
        public static void UpdateData<T>([NotNull] List<T> list, string dataPath) where T : ScriptableObjectData
        {
            float startTime = Time.time;

            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            list.Clear();

            string[] assetGuids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new string[]
            {
                dataPath
            });

            foreach (string guid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T data = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (data != null)
                {
                    list.Add(data);
                }
            }

            Log.Write($"Data updated in {Time.time - startTime} seconds!");
        }
#endif

        public static Vector3 GetScaleBasedOnTextureSize(float width, float height, float upscaleFactor = 1f)
        {
            float aspect = width / height;

            return new Vector3(aspect, 1, 1) * upscaleFactor;
        }

#if UNITY_EDITOR
        public static bool GetKeyPressed(out KeyCode keyCode)
        {
            keyCode = KeyCode.None;
            var @event = Event.current;

            if (!@event.isKey)
            {
                return false;
            }

            if (@event.type != EventType.KeyDown)
            {
                return false;
            }

            if (@event.keyCode == lastKeyPressed)
            {
                return false;
            }

            keyCode = @event.keyCode;
            lastKeyPressed = keyCode;

            return true;
        }
#endif
    }
}
