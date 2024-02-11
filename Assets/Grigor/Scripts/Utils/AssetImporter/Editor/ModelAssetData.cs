using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RazerCore.Utils.AssetImporter.Editor
{
    [Serializable]
    public class ModelAssetData
    {
        [SerializeField, HideLabel] private Material material;
        [SerializeField, ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true)] private List<MaterialAssets> materialAssets;

        public Material Material => material;
        public List<MaterialAssets> MaterialAssets => materialAssets;

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
