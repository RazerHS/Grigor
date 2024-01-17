#if UNITY_EDITOR

using Grigor.Characters;
using Grigor.Data.Editor;
using UnityEditor;

namespace RazerCore.Utils.Editor
{
    public class CharacterEditorWindow : AssetCreatorWindow<CharacterData>
    {
        protected override string dataName => "Character";
        protected override string dataAssetsPath => "Assets/Grigor/Data/Characters";

        [MenuItem("Grigor/Characters")]
        protected static void OpenWindow()
        {
            GetWindow<CharacterEditorWindow>().Show();
        }
    }
}

#endif
