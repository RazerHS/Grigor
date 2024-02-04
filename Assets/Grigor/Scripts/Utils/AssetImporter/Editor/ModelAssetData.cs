using System;
using System.Collections.Generic;
using UnityEngine;

namespace RazerCore.Utils.AssetImporter.Editor
{
    [Serializable]
    public class ModelAssetData
    {
        [SerializeField] private Material material;
        [SerializeField] private List<MaterialAssets> materialAssets;

        public Material Material => material;

        public void SetMaterial(Material material)
        {
            this.material = material;
        }

        public void SetMaterialAssets(List<MaterialAssets> materialAssets)
        {
            this.materialAssets = materialAssets;
        }
    }
}
