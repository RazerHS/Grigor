//***************************************************************************
//* Copyright (C) Tech Gorillas Ltd  2017-2019 - All Rights Reserved
//* Unauthorized copying of this file, via any medium is strictly prohibited
//* Proprietary and confidential
//* Written by Bruce Heather <bruce@tech-gorillas.com>, January 2019 */
//***************************************************************************

using UnityEditor;
using UnityEngine;

namespace Gorillas.Editor.Navigation
{
    /// <summary>
    /// This class implements the Unity Version specific code
    /// </summary>
    public class SceneHistory : SceneHistoryBase
    {
        [MenuItem("Window/Shortcuts Suite/Scene History")]
        static void SceneHistorySpawnWindow()
        {
            GetWindow(typeof(SceneHistory), false, "Scene History");
        }

	    public override Object GetCorresdpondingItemInPrefab(GameObject go)
	    {
#if UNITY_2018_2_OR_NEWER
			if (PrefabUtility.GetPrefabInstanceStatus(go) == PrefabInstanceStatus.NotAPrefab)
			{
				return null;
			}
			GameObject targetPrefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
#else
		    Object targetPrefab = PrefabUtility.GetPrefabParent(go);
#endif
		    return targetPrefab;
	    }

	    public override GameObject GetRootOfNearestPrefab(GameObject go)
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
	}
}
