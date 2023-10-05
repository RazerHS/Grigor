using Sirenix.Utilities;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    [GlobalConfig, CreateAssetMenu(fileName = "LightingConfig", menuName = "Grigor/LightingConfig")]
    public class LightingConfig : GlobalConfig<LightingConfig>
    {
        [SerializeField] private Gradient ambientColor;
        [SerializeField] private Gradient directionalColor;
        [SerializeField] private Gradient fogColor;

        public Gradient AmbientColor => ambientColor;
        public Gradient DirectionalColor => directionalColor;
        public Gradient FogColor => fogColor;
    }
}
