using System;
using System.Collections.Generic;
using System.Reflection;
using CardboardCore.DI.Interfaces;
using CardboardCore.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CardboardCore.DI
{
    /// <summary>
    /// Handles the actual injection into and dumping from fields. Multiple of these layers can co-exist.
    /// Keeps track of references to specific injections and cleans them up when being dumped.
    /// </summary>
    public class InjectionLayer
    {
	    private readonly Dictionary<Type, object> singletonDependencies = new Dictionary<Type, object>();
	    private readonly Dictionary<object, List<object>> singletonReferences = new Dictionary<object, List<object>>();
	    private readonly Dictionary<object, object> nonSingletonReferences = new Dictionary<object, object>();
	    private readonly List<object> singletons = new List<object>();

        public void InjectIntoField(FieldInfo fieldInfo, InjectableAttribute injectableAttribute, object @object, bool firstTry = true)
        {
            object injectedInstance;

			if(injectableAttribute.Singleton)
			{
				if(singletonDependencies.ContainsKey(fieldInfo.FieldType))
				{
					injectedInstance = singletonDependencies[fieldInfo.FieldType];
				}
				else
				{
					if(fieldInfo.FieldType.IsSubclassOf(typeof(MonoBehaviour)))
					{
						injectedInstance = Object.FindObjectOfType(fieldInfo.FieldType, true);

						if(injectedInstance == null)
						{
							GameObject newGameObject = new GameObject
							{
								name = fieldInfo.FieldType.Name
							};

							injectedInstance = newGameObject.AddComponent(fieldInfo.FieldType);
						}

						if(((MonoBehaviour)injectedInstance).gameObject.GetComponentsInChildren(typeof(MonoBehaviour), true).Length > 1)
						{
							Log.Warn($"Found multiple MonoBehaviours on {fieldInfo.FieldType}. " +
								"Check the hierarchy as some components may be moved unintentionally");
						}

						if(Application.isPlaying)
						{
							Object.DontDestroyOnLoad(((MonoBehaviour)injectedInstance).gameObject);
						}
					}
					else
					{
						injectedInstance = Activator.CreateInstance(fieldInfo.FieldType);
					}
				}

				if(injectedInstance == null || injectedInstance.ToString().Equals("null"))
				{
					if(!firstTry && singletonDependencies.ContainsKey(fieldInfo.FieldType))
					{
						Log.Warn($"Re-mapping instance of Type {fieldInfo.FieldType}...");

						singletonDependencies.Remove(fieldInfo.FieldType);
						InjectIntoField(fieldInfo, injectableAttribute, @object, false);

						return;
					}

					throw Log.Exception($"Something went wrong while trying to inject <b>{fieldInfo.FieldType}</b>");
				}

				singletonDependencies[fieldInfo.FieldType] = injectedInstance;

				// Track all singletons which aren't allowed to be auto-destroyed
				if(!singletons.Contains(injectedInstance) && !injectableAttribute.ClearAutomatically)
				{
					singletons.Add(injectedInstance);
				}

				// Track references to this singleton injected instance
				if(singletonReferences.ContainsKey(injectedInstance))
				{
					if(singletonReferences[injectedInstance].Contains(@object))
					{
						Log.Warn(
							"Trying to inject the same Object of type " +
							$"<b>{fieldInfo.FieldType}</b> twice in <b>{@object}</b>");

						return;
					}

					singletonReferences[injectedInstance].Add(@object);
				}
				else
				{
					singletonReferences[injectedInstance] = new List<object> { @object };

					if(injectedInstance is DIInitializable initializable)
					{
						initializable.Initialize();
					}
				}
			}
			else
			{
				if(nonSingletonReferences.ContainsKey(@object))
				{
					Log.Warn(
						"Trying to inject the same Object of type " +
						$"<b>{fieldInfo.FieldType}</b> twice in <b>{@object}</b>");

					return;
				}

				if(fieldInfo.FieldType.IsSubclassOf(typeof(MonoBehaviour)))
				{
					throw Log.Exception("Non-Singleton Monobehaviour support is not added yet!"
						+ $"Please change <b>{fieldInfo.FieldType.Name}</b> to be a <b>Singleton</b> Injectable");
				}

				injectedInstance = Activator.CreateInstance(fieldInfo.FieldType);

				nonSingletonReferences[@object] = injectedInstance;

				Log.Write($"Non-Singleton <b>{injectedInstance.GetType().Name}</b> was created");

				if(injectedInstance is DIInitializable initializable)
				{
					initializable.Initialize();
				}
			}

			fieldInfo.SetValue(@object, injectedInstance);

			if(injectableAttribute.Singleton)
			{
				Log.Write($"Injected <i>Singleton</i> instance <b>{injectedInstance.GetType().Name}</b> " +
					$"into <b>{@object.GetType().Name}</b> -- " +
					$"Has <b>{singletonReferences[injectedInstance].Count}</b> reference(s)");
			}
			else
			{
				Log.Write($"Created instance <b>{injectedInstance.GetType().Name}</b> -- " +
					$"Was injected into <b>{@object.GetType().Name}</b>");
			}
        }

        public void ReleaseDependencies(object @object)
        {
	        List<object> instancesToDestroy = new List<object>();

			if(nonSingletonReferences.TryGetValue(@object, out object obj))
			{
				instancesToDestroy.Add(obj);
			}
			else
			{
				foreach(KeyValuePair<object, List<object>> keyValuePair in singletonReferences)
				{
					object injectedInstance = keyValuePair.Key;

					List<object> objects = keyValuePair.Value;

					for(int i = 0; i < objects.Count; i++)
					{
						if(objects[i] != @object)
						{
							continue;
						}

						objects.RemoveAt(i);

						Log.Write($"<i>Singleton</i> instance <b>{injectedInstance.GetType().Name}</b> -- " +
							$"Released by <b>{@object.GetType().Name}</b> -- " +
							$"Has <b>{singletonReferences[injectedInstance].Count}</b> reference(s)");

						break;
					}

					// Injected instance has no more references, time to clean it up
					if(objects.Count == 0)
					{
						Type type = injectedInstance.GetType();

						if(singletons.Contains(injectedInstance))
						{
							continue;
						}

						singletonDependencies.Remove(type);
						instancesToDestroy.Add(injectedInstance);
					}
				}
			}


			// Finally remove references by key
			for(int i = 0; i < instancesToDestroy.Count; i++)
			{
				Log.Write(
					"Clearing instance of " +
					$"<b>{instancesToDestroy[i].GetType().Name}</b> as it has no more references left");

				singletonReferences.Remove(instancesToDestroy[i]);

				if(instancesToDestroy[i].GetType().IsSubclassOf(typeof(MonoBehaviour)))
				{
					MonoBehaviour monoBehaviour = (MonoBehaviour)instancesToDestroy[i];

					if(instancesToDestroy[i] is DIDisposable disposable)
					{
						disposable.Dispose();
					}

					// Only destroy the gameobject if in play mode, in editor mode we generally don't want this to happen
					if(Application.isPlaying)
					{
						Object.Destroy(monoBehaviour.gameObject);
					}
				}
				else
				{
					if(instancesToDestroy[i] is DIDisposable disposable)
					{
						disposable.Dispose();
					}

					instancesToDestroy[i] = null;
				}
			}
        }
    }
}
