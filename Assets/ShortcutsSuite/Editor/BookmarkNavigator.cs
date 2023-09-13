//***************************************************************************
//* Copyright (C) Tech Gorillas Ltd  2017-2019 - All Rights Reserved
//* Unauthorized copying of this file, via any medium is strictly prohibited
//* Proprietary and confidential
//* Written by Bruce Heather <bruce@tech-gorillas.com>, January 2019 */
//***************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gorillas.Editor.Navigation
{
    /// <summary>
    /// This class implements the BookmarkNavigator late binding code
    /// </summary>
    public class BookmarkNavigator : BookmarkNavigatorBase
    {
	    private static BookmarkNavigator _window = null;

		[MenuItem("Window/Shortcuts Suite/Scene Bookmarks")]
	    static void FindObjectRegisterBookmarkWindow()
	    {
		    _window = (BookmarkNavigator)GetWindow(typeof(BookmarkNavigator), false, "Scene Bookmarks");
		    if (_window == null)
		    {
			    Debug.LogError("Unable to open Scene Bookmarks");
		    }
	    }

	    [MenuItem("Window/Shortcuts Suite/Add Bookmark &#b", false, 33)]
	    [MenuItem("Assets/Shortcuts Suite/Add Bookmark", false, 33)]
	    [MenuItem("GameObject/Shortcuts Suite/Add Bookmark", false, 33)]
	    public static void AddBookmark(MenuCommand command)
	    {
		    if (_window == null)
		    {
			    _window = (BookmarkNavigator)GetWindow(typeof(BookmarkNavigator), false, "Scene Bookmarks");
		    }

			if (command.context != null)
		    {
			    _window.AddBookmark(command.context);
			}
			else
		    {
			    _window.AddBookmark(Selection.activeObject);
		    }
	    }

		protected override Guid GetGuid(GameObject go, out bool add)
        {
            Guid searchGuid = Guid.Empty;
	        add = false;
            if (!string.IsNullOrEmpty(go.scene.name)) // THis is a scene object
            {
	            GuidComponent or = go.GetComponent<GuidComponent>();
	            if (or != null) // which already has a GUId
	            {
		            searchGuid = or.GetGuid();
		            or.Increment();
	            }
				else if (!EditorApplication.isPlaying &&
						!EditorApplication.isPlayingOrWillChangePlaymode &&
						!EditorApplication.isTemporaryProject &&
						!EditorApplication.isUpdating) // Only add a guid if not in play mode as it will be lost when you exit play mode
	            {
		            add = true;
					or = go.AddComponent<GuidComponent>();
		            or.Increment();
		            searchGuid = or.GetGuid();
	            }
			}
            return searchGuid;
        }

	    protected override void RemoveGuid(GameObject go)
	    {
		    GuidComponent toRemove = go.GetComponent<GuidComponent>();
		    DestroyImmediate(toRemove);
	    }

		protected override List<GameObject> FindMatchingGameObjects(Guid tag)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            var objTags = Resources.FindObjectsOfTypeAll<GuidComponent>();
            // highlight ones that match the name.
            var highlight = objTags.Where(obj => obj.GetGuid().Equals(tag));
            foreach (var h in highlight)
            {
                if (h.gameObject.scene.name != null)
                {
                    gameObjects.Add(h.gameObject);
                }
            }
            return gameObjects;
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