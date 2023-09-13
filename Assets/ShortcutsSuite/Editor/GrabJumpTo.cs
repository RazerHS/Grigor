#region COPYRIGHT

// ***************************************************************************
// * Copyright (C) Tech Gorillas Ltd  2017-2020 - All Rights Reserved
// * Unauthorized copying of this file, via any medium is strictly prohibited
// * Proprietary and confidential
// * Written by Bruce Heather <bruce@tech-gorillas.com>, 07 2020*/
// ***************************************************************************

#endregion

using System.Collections.Generic;
using System.Linq;
using Gorillas.Editor;
using UnityEditor;
using UnityEngine;
using Gorillas;

namespace Assets.Shortcuts_Suite.Editor
{
	public class GrabJumpTo
	{
		private static string _jumpToPath = null;

		[MenuItem("Assets/Shortcuts Suite/JumpTo", false, 20)]
		[MenuItem("Window/Shortcuts Suite/JumpTo %j", false, 20)]
		[MenuItem("GameObject/Shortcuts Suite/JumpTo", false, 20)]
		public static void JumpTo()
		{
			//string path = Clipboard.Paste<string>();
			string path = EditorGUIUtility.systemCopyBuffer;
			path = path.Replace("\\\\", "/").Replace("\\", "/").Replace(Application.dataPath, "Assets");
			Transform targetTransform = null;
			if (!NavigationBase.FindByPath(path, out targetTransform))
			{
				NavigationBase.FindByPath(_jumpToPath, out targetTransform);
			}
			if (targetTransform == null)
			{
				Object[] array = {AssetDatabase.LoadAssetAtPath(path, typeof(Object))};
				if (array == null || array.Length == 0)
				{
					array = new[] {AssetDatabase.LoadAssetAtPath(_jumpToPath, typeof(Object))};
				}
				Selection.objects = array;

				EditorGUIUtility.PingObject(array[0]);
				if (SceneView.lastActiveSceneView != null)
				{
					SceneView.lastActiveSceneView.FrameSelected();
				}
			}
			else
			{
				Selection.objects = new Object[] {targetTransform.gameObject};
			}

			if (Selection.objects != null && Selection.objects.Length > 0)
			{
				EditorGUIUtility.PingObject(Selection.objects[0]);
			}
		}

		[MenuItem("Assets/Shortcuts Suite/Copy Full Path")]
		public static void CopyFullPath(MenuCommand command)
		{
			if (Selection.activeGameObject == null)
			{
				_jumpToPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + AssetDatabase.GetAssetPath(Selection.activeObject);
				EditorGUIUtility.systemCopyBuffer = _jumpToPath.Replace("/", "\\");
			}
			else
			{
				_jumpToPath = FullPath(Selection.activeGameObject);
				EditorGUIUtility.systemCopyBuffer = _jumpToPath;
			}
		}


		[MenuItem("Window/Shortcuts Suite/Grab Path %g", false, 20)]
		[MenuItem("Assets/Shortcuts Suite/Grab Path", false, 20)]
		[MenuItem("GameObject/Shortcuts Suite/Grab Path", false, 20)]
		public static void CopyPath(MenuCommand command)
		{
			Object target = (command == null || command.context == null) ? Selection.activeGameObject : command.context;
			GameObject context = target as GameObject;
			if (context != null)
			{
				//Clipboard.Copy(FullPath(command.context as GameObject));
				_jumpToPath = FullPath(context);
				EditorGUIUtility.systemCopyBuffer = _jumpToPath;
			}
			else
			{
				_jumpToPath = AssetDatabase.GetAssetPath(Selection.activeObject);
				EditorGUIUtility.systemCopyBuffer = _jumpToPath;
			}
		}

		[MenuItem("Window/Shortcuts Suite/Select Prefab Root &#r", false, 20)]
		[MenuItem("GameObject/Shortcuts Suite/Select Prefab Root", false, 20)]
		public static void SelectPrefab(MenuCommand command)
		{
			Object target = (command == null || command.context == null) ? Selection.activeGameObject : command.context;
			GameObject context = target as GameObject;
			if (context != null)
			{
				Object prefab = GetRootOfOutsidePrefab(context);
				Object[] array = { prefab };
				Selection.objects = array;
				EditorGUIUtility.PingObject(prefab);
			}
		}

#if UNITY_2018_2_OR_NEWER
		[MenuItem("Window/Shortcuts Suite/Select Nearest Prefab &#p", false, 20)]
		[MenuItem("GameObject/Shortcuts Suite/Select Nearest Prefab", false, 20)]
		public static void SelectNearestPrefab(MenuCommand command)
		{
			Object target = (command == null || command.context == null) ? Selection.activeGameObject : command.context;
			GameObject context = target as GameObject;
			if (context != null)
			{
				Object prefab = GetRootOfNearestPrefab(context);
				Object[] array = { prefab };
				Selection.objects = array;
				EditorGUIUtility.PingObject(prefab);
			}
		}
#endif

		private static string FullPath(GameObject go)
		{
			return go.transform.parent == null
				? go.scene.name == null ? "" : go.scene.name + "/" + go.name
				: FullPath(go.transform.parent.gameObject) + "/" + go.name;
		}


		public static GameObject GetRootOfNearestPrefab(GameObject go)
		{
#if UNITY_2018_2_OR_NEWER
			if (PrefabUtility.GetPrefabInstanceStatus(go) == PrefabInstanceStatus.NotAPrefab)
			{
				return null;
			}
			GameObject targetPrefab = PrefabUtility.GetNearestPrefabInstanceRoot(go);
			return targetPrefab;
#else
			Object prefabObject = PrefabUtility.GetPrefabObject(go);
			if (prefabObject == null)
				return null;
			return PrefabUtility.FindPrefabRoot(go);
#endif
		}

		public static GameObject GetRootOfOutsidePrefab(GameObject go)
		{
#if UNITY_2018_2_OR_NEWER
			PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(go);
			if (prefabInstanceStatus == PrefabInstanceStatus.NotAPrefab)
			{
				return null;
			}
			GameObject targetPrefab = go.scene.name == null ? go.transform.root.gameObject : PrefabUtility.GetOutermostPrefabInstanceRoot(go);
			return targetPrefab;
#else
			Object prefabObject = PrefabUtility.GetPrefabObject(go);
			if (prefabObject == null)
				return null;
			return PrefabUtility.FindPrefabRoot(go);
#endif
		}
	}
}