using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Editor
{
    public class ScriptableObjectData : ScriptableObject
    {
        [SerializeField, HorizontalGroup("name", PaddingLeft = 3), ColoredBoxGroup("name/Asset Name", false, 0.9f, 0.1f, 0.5f), HideLabel] protected string assetName;

        public string AssetName => assetName;
    }
}
