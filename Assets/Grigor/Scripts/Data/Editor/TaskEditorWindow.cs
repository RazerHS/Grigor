#if UNITY_EDITOR

using Grigor.Data.Tasks;
using UnityEditor;

namespace Grigor.Data.Editor
{
    public class TaskEditorWindow : AssetCreatorWindow<TaskData>
    {
        protected override string dataName => "Task";
        protected override string dataAssetsPath => "Assets/Grigor/Data/Tasks";

        [MenuItem("Grigor/Tasks")]
        protected static void OpenWindow()
        {
            GetWindow<TaskEditorWindow>().Show();
        }
    }
}

#endif
