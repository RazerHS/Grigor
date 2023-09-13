using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

#if CC_DI
using CardboardCore.DI;
using CardboardCore.Utilities;
#endif

namespace CardboardCore.Pooling
{
#if CC_DI
	[Injectable]
#endif
	public class PoolManager
	{
		private readonly Dictionary<string, AsyncOperationHandle<PoolConfig>> handles = new Dictionary<string, AsyncOperationHandle<PoolConfig>>();
		private readonly Dictionary<string, Transform> containers = new Dictionary<string, Transform>();
		private readonly Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

		private async Task<Pool> CreateNewPool(string poolConfigName, Transform container)
		{
			// Check if we're already loading the pool config we need
			bool createNewPool = !handles.ContainsKey(poolConfigName);

			AsyncOperationHandle<PoolConfig> asyncOperationHandle;

			// Load the pool config asset if a new pool is required
			if(createNewPool)
			{
				asyncOperationHandle = Addressables.LoadAssetAsync<PoolConfig>(poolConfigName);
				handles[poolConfigName] = asyncOperationHandle;
			}
			else
			{
				asyncOperationHandle = handles[poolConfigName];
			}

			await asyncOperationHandle.Task;

			Pool newPool;

			// Create a new pool once loading has finished
			if(createNewPool)
			{
				newPool = new Pool(asyncOperationHandle.Result, container);
				pools[poolConfigName] = newPool;

#if CC_DI
				Log.Write($"New Pool created with Config \"{poolConfigName}\"");
#else
				Debug.Log($"New Pool created with Config \"{poolConfigName}\"");
#endif
			}
			// Grab the newly created pool in the case we wanted a new pool but once loading has finished
			else
			{
				if(!pools.TryGetValue(poolConfigName, out newPool))
				{
#if CC_DI
					throw Log.Exception("Something went wrong wait for pool to be created");
#else
					throw new Exception("Something went wrong wait for pool to be created");
#endif
				}

#if CC_DI
				Log.Write($"Waited for another task to finish creation of Pool with Config \"{poolConfigName}\"");
#else
				Debug.Log($"Waited for another task to finish creation of Pool with Config \"{poolConfigName}\"");
#endif
			}

			return newPool;
		}

		/// <summary>
		/// Will initialize a pool if it's not created yet.
		/// Best timing to do this is during initialization moments.
		/// </summary>
		/// <param name="poolConfigName"></param>
		/// <returns></returns>
		public async Task<Pool> RequestPool(string poolConfigName)
		{
			if (!containers.TryGetValue(poolConfigName, out Transform container))
			{
				GameObject poolContainerGameObject = new GameObject();
				poolContainerGameObject.name = $"PoolContainer-{poolConfigName}";

				Object.DontDestroyOnLoad(poolContainerGameObject);

				container = poolContainerGameObject.transform;

				containers[poolConfigName] = container;
			}

			if(pools.TryGetValue(poolConfigName, out Pool pool))
			{
				return pool;
			}

			return await CreateNewPool(poolConfigName, containers[poolConfigName]);
		}

		public void KillPool(string poolConfigName)
		{
			if (!handles.TryGetValue(poolConfigName, out AsyncOperationHandle<PoolConfig> handle)
				|| !containers.TryGetValue(poolConfigName, out Transform container)
				|| !pools.TryGetValue(poolConfigName, out Pool pool))
			{
#if CC_DI
				Log.Write($"Unable to kill pool with name {poolConfigName}");
#else
				Debug.LogError($"Unable to kill pool with name {poolConfigName}");
#endif
				return;
			}

			Addressables.Release(handle);
			handles.Remove(poolConfigName);

			Object.Destroy(container.gameObject);
			containers.Remove(poolConfigName);

			pools.Remove(poolConfigName);
		}
	}
}
