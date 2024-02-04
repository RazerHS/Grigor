using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RazerCore.Utils.AssetImporter.Editor
{
    [Serializable]
    public class ShaderPropertySelector
    {
        [SerializeField, HorizontalGroup("property", Width = 0.9f), HideLabel, ReadOnly] private string property;
        [SerializeField, HorizontalGroup("property", Width = 0.1f), HideLabel] private bool enable = true;

        public string Property => property;
        public bool Enable => enable;

        public ShaderPropertySelector(string property)
        {
            this.property = property;
        }

        public void SetProperty(string property)
        {
            this.property = property;
        }

        public void SetEnabled(bool enable)
        {
            this.enable = enable;
        }
    }
}
