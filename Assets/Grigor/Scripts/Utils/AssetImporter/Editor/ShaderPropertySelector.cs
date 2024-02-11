using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RazerCore.Utils.AssetImporter.Editor
{
    [Serializable]
    public class ShaderPropertySelector
    {
        [SerializeField, HorizontalGroup("property", Width = 0.9f), HideLabel, ReadOnly, DisplayAsString(EnableRichText = true, FontSize = 15)] private string property;
        [SerializeField, HorizontalGroup("property"), VerticalGroup("property/vertical", PaddingTop = 2), HideLabel] private bool enable = true;

        public string Property => property;
        public bool Enable => enable;

        public ShaderPropertySelector(string property)
        {
            this.property = property;
        }

        public void SetProperty(string property)
        {
            this.property = $"<b>{property}</b>";
        }

        public void SetEnabled(bool enable)
        {
            this.enable = enable;
        }
    }
}
