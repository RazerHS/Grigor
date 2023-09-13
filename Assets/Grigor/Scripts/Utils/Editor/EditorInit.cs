using UnityEditor;
using UnityEditor.SceneManagement;

namespace Grigor.Utils.Editor
{
    [InitializeOnLoad]
    public class EditorInit
    {
        static EditorInit()
        {
            string pathOfFirstScene = EditorBuildSettings.scenes[0].path;
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);

            EditorSceneManager.playModeStartScene = sceneAsset;
        }
    }
}
