﻿using Grigor.Characters;
using Grigor.Data.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
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

        protected override void OnDrawTree(OdinMenuTree tree)
        {
            tree.EnumerateTree().ForEach(AddDragHandles);
        }
    }
}
