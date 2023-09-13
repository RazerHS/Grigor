using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if CC_DI
using CardboardCore.Utilities;
#endif

namespace CardboardCore.Pooling
{
	public class Pool
	{
		private readonly PoolConfig poolConfig;
		private readonly Transform poolContainer;

		private readonly Dictionary<string, Stack<MonoBehaviour>> _instances = new Dictionary<string, Stack<MonoBehaviour>>();

		private bool _isPopulated;

		public string Id => poolConfig.name;

		public Pool(PoolConfig poolConfig, Transform poolContainer)
		{
			this.poolConfig = poolConfig;
			this.poolContainer = poolContainer;

			TryPopulate();
		}

		private void TryPopulate()
		{
			if(_isPopulated)
			{
				return;
			}

			if(poolConfig.PoolEntries.Length > 0 && _instances.Count == 0)
			{
				Populate();
				_isPopulated = true;
			}
		}

		private void Populate()
		{
			for(int i = 0; i < poolConfig.PoolEntries.Length; i++)
			{
				PoolEntry poolEntry = poolConfig.PoolEntries[i];

				for(int k = 0; k < poolEntry.InitialAmount; k++)
				{
					MonoBehaviour instance = Object.Instantiate(poolEntry.Prefab);
					instance.name = poolEntry.Prefab.name;

					Push(instance, false);
				}
			}
		}

		public T Pop<T>(string instanceName, Transform parent = null) where T : MonoBehaviour
		{
			TryPopulate();

			if(!_instances.ContainsKey(instanceName))
			{
				throw new Exception($"No PoolEntry found for <b>{instanceName}</b>");
			}

			Stack<MonoBehaviour> stack = _instances[instanceName];
			MonoBehaviour instance = stack.Pop();
			T typedInstance = instance as T;

			if(typedInstance == null)
			{
				typedInstance = instance.gameObject.GetComponent<T>();

				if(typedInstance == null)
				{
					throw new Exception($"No Instance found for <b>{instanceName}</b> of type <b>{typeof(T)}</b>");
				}

#if CC_DI
				Log.Warn("Found pooled instance via GetComponent, to improve performance, try to assign a prefab to the PoolConfig with the type you want.");
#else
				Debug.LogWarning("Found pooled instance via GetComponent, to improve performance, try to assign a prefab to the PoolConfig with the type you want.");
#endif
			}

			if(stack.Count == 0)
			{
				for(int i = 0; i < poolConfig.PoolEntries.Length; i++)
				{
					if(poolConfig.PoolEntries[i].Prefab.name.Equals(instance.gameObject.name))
					{
						for(int j = 0; j < poolConfig.PoolEntries[i].PoolScaleAmount; j++)
						{
							MonoBehaviour newInstance = Object.Instantiate(instance);
							newInstance.name = instanceName;
							Push(newInstance, false);
						}

						break;
					}
				}
			}

			if(instance.transform is RectTransform rectTransform)
			{
				rectTransform.SetParent(parent, false);
			}
			else
			{
				instance.transform.parent = parent;
			}

			instance.gameObject.SetActive(true);

			if(instance is IPoolable poolable)
			{
				poolable.OnPop();
			}

			return typedInstance;
		}

		public void Push<T>(T instance, bool callInterfaceMethod = true) where T : MonoBehaviour
		{
			if(instance == null)
			{
				return;
			}

			if(!_instances.ContainsKey(instance.name))
			{
				_instances[instance.name] = new Stack<MonoBehaviour>();
			}

			Stack<MonoBehaviour> stack = _instances[instance.name];

			if(callInterfaceMethod && instance is IPoolable poolable)
			{
				poolable.OnPush();
			}

			instance.gameObject.SetActive(false);

			if(instance.transform is RectTransform rectTransform)
			{
				rectTransform.SetParent(poolContainer, false);
			}
			else
			{
				instance.transform.parent = poolContainer;
			}

			stack.Push(instance);
		}
	}
}
