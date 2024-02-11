using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RazerCore.Utils.AssetImporter.Editor
{
    [Serializable]
    public class MaterialAssets
    {
        [SerializeField, HorizontalGroup("Assets", Width = 0.6f), VerticalGroup("Assets/Vertical"), HideLabel, DisplayAsString(EnableRichText = true, FontSize = 10)] private string property;
        [SerializeField, HorizontalGroup("Assets"), VerticalGroup("Assets/Vertical"), HideLabel, DisplayAsString(FontSize = 10)] private string textureName;
        [SerializeField, HorizontalGroup("Assets/Right"), PreviewField(Alignment = ObjectFieldAlignment.Right), HideLabel] private Texture2D texture;

        public string Property => property.Replace("<b>", string.Empty).Replace("</b>", string.Empty);
        public Texture2D Texture => texture;

        public void SetProperty(string property)
        {
            this.property = $"<b>{property}</b>";
        }

        public void SetTexture(Texture2D texture)
        {
            if (texture == null)
            {
                this.texture = null;
                textureName = string.Empty;

                return;
            }

            this.texture = texture;

            textureName = texture.name;
        }
    }
}
