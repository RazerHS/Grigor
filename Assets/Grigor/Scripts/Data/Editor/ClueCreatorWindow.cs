using Grigor.Data.Clues;
using Grigor.Data.Editor;
using UnityEditor;

namespace RazerCore.Utils.Editor
{
    public class ClueEditorWindow : AssetCreatorWindow<ClueData>
    {
        protected override string dataName => "Clue";
        protected override string dataAssetsPath => "Assets/Grigor/Data/Clues";

        [MenuItem("Grigor/Clues")]
        protected static void OpenWindow()
        {
            GetWindow<ClueEditorWindow>().Show();
        }
    }
}
