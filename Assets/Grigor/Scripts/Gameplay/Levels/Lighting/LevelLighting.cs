using Grigor.Gameplay.Time.Lighting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Grigor.Gameplay.Time.Lighting
{
    public class LevelLighting : MonoBehaviour
    {
        [SerializeField] private SunController sunController;
        [SerializeField] private Volume skyAndFogVolume;
        [SerializeField] private Volume postProcessingVolume;

        public SunController SunController => sunController;
        public Volume SkyAndFogVolume => skyAndFogVolume;
        public Volume PostProcessingVolume => postProcessingVolume;
    }
}
