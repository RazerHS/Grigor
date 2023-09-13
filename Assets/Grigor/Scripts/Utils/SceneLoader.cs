using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Grigor.Utils
{
    public static class SceneLoader
    {
        private static readonly List<string> loadingScenes = new();
        private static readonly List<string> unloadingScenes = new();

        private static string sceneNameToSetActive;

        private static Action loadingFinished;
        private static Action unloadingFinished;

        static SceneLoader()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            loadingScenes.Remove(scene.name);
            CheckForLoadingFinished();

            if (sceneNameToSetActive == scene.name)
            {
                SceneManager.SetActiveScene(scene);
            }
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            unloadingScenes.Remove(scene.name);
            CheckForUnloadingFinished();
        }

        private static void LoadScene(string name, LoadSceneMode loadSceneMode)
        {
            loadingScenes.Add(name);
            SceneManager.LoadSceneAsync(name, loadSceneMode);
        }

        private static void UnloadScene(string name)
        {
            unloadingScenes.Add(name);
            SceneManager.UnloadSceneAsync(name);
        }

        private static void CheckForLoadingFinished()
        {
            CheckFinished(loadingScenes, ref loadingFinished);
        }

        private static void CheckForUnloadingFinished()
        {
            CheckFinished(unloadingScenes, ref unloadingFinished);
        }

        private static void CheckFinished(List<string> sceneNames, ref Action onComplete)
        {
            if (sceneNames.Count > 0)
            {
                return;
            }

            onComplete?.Invoke();
            onComplete = null;
        }

        public static void LoadScenes(bool setFirstSceneAsActive, params string[] sceneNames)
        {
            sceneNameToSetActive = setFirstSceneAsActive ? sceneNames[0] : null;

            for (int i = 0; i < sceneNames.Length; i++)
            {
                LoadScene(sceneNames[i], LoadSceneMode.Additive);
            }
        }

        public static void LoadScenes(Action onCompleted, bool setFirstSceneAsActive, params string[] sceneNames)
        {
            loadingFinished = onCompleted;
            LoadScenes(setFirstSceneAsActive, sceneNames);
        }

        public static void UnloadScenes(params string[] sceneNames)
        {
            for (int i = 0; i < sceneNames.Length; i++)
            {
                UnloadScene(sceneNames[i]);
            }
        }

        public static void UnloadScenes(Action onCompleted, params string[] sceneNames)
        {
            unloadingFinished = onCompleted;
            UnloadScenes(sceneNames);
        }

        public static void UnloadAllScenes(Action onCompleted, params string[] scenesToSkip)
        {
           unloadingFinished = onCompleted;

            int unloadSceneCount = 0;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scenesToSkip.Contains(scene.name))
                {
                    continue;
                }

                UnloadScene(scene.name);
                unloadSceneCount++;
            }

            if (unloadSceneCount == 0)
            {
                CheckForUnloadingFinished();
            }
        }
    }
}
