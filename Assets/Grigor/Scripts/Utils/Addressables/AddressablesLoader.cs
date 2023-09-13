using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardboardCore.DI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Grigor.Addressables
{
    [Injectable]
    public class AddressablesLoader
    {
        private readonly Dictionary<string, AsyncOperationHandle> dict = new Dictionary<string, AsyncOperationHandle>();

        public async void LoadAssetAsync<T>(string assetName, Action<T> callback) where T : Object
        {
            if (dict.TryGetValue(assetName, out AsyncOperationHandle value))
            {
                callback?.Invoke(value.Result as T);
                return;
            }

            AsyncOperationHandle<T> asyncOperationHandle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(assetName);

            Task<T> task = asyncOperationHandle.Task;
            await task;

            dict.Add(assetName, asyncOperationHandle);

            callback?.Invoke(task.Result);
        }

        public async void LoadSceneAsync(string sceneName, Action<SceneInstance> callback)
        {
            if (dict.TryGetValue(sceneName, out AsyncOperationHandle value))
            {
                callback?.Invoke((SceneInstance)value.Result);
                return;
            }

            AsyncOperationHandle<SceneInstance> loadSceneAsync = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(
                sceneName, LoadSceneMode.Additive, true);

            Task<SceneInstance> task = loadSceneAsync.Task;
            await task;

            dict.Add(sceneName, loadSceneAsync);

            callback?.Invoke(task.Result);
        }

        public async void UnloadSceneAsync(string sceneName, Action callback)
        {
            if (dict.TryGetValue(sceneName, out AsyncOperationHandle asyncOperationHandle))
            {
                await UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(asyncOperationHandle).Task;
                dict.Remove(sceneName);
            }

            callback?.Invoke();
        }

        public void ClearAll()
        {
            foreach ((string assetName, AsyncOperationHandle asyncOperationHandle) in dict)
            {
                UnityEngine.AddressableAssets.Addressables.Release(asyncOperationHandle);
            }

            dict.Clear();
        }
    }
}
