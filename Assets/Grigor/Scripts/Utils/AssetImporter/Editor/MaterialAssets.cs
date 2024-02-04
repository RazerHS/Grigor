using System;
using UnityEngine;

namespace RazerCore.Utils.AssetImporter.Editor
{
    [Serializable]
    public class MaterialAssets
    {
        [SerializeField] private string property;
        [SerializeField] private Texture2D texture;

        public string Property => property;
        public Texture2D Texture => texture;

        public void SetProperty(string property)
        {
            this.property = property;
        }

        public void SetTexture(Texture2D texture)
        {
            this.texture = texture;
        }
    }
}
