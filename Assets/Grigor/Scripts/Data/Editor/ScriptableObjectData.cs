using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Grigor.Data.Editor
{
    public class ScriptableObjectData : ScriptableObject
    {
        [SerializeField, HorizontalGroup("name", PaddingLeft = 3), ColoredBoxGroup("name/Asset Name", false, 0.9f, 0.1f, 0.5f), HideLabel, DisableIf(nameof(nameLocked))] protected string assetName;
        [HorizontalGroup("name", Width = 0.05f), Button(SdfIconType.UnlockFill, nameof(UnlockAssetName)), ShowIf(nameof(nameLocked))] private void UnlockName() => UnlockAssetName();
        [HorizontalGroup("name", Width = 0.05f), Button(SdfIconType.LockFill, nameof(LockAssetName)), HideIf(nameof(nameLocked))] private void LockName() => LockAssetName();

        private bool nameLocked = true;

        public string AssetName => assetName;

        private void LockAssetName()
        {
            nameLocked = true;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), assetName);
        }

        private void UnlockAssetName()
        {
            nameLocked = false;
        }
    }
}
