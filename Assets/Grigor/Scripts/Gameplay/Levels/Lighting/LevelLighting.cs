using Grigor.Gameplay.Weather;
using UnityEngine;
using UnityEngine.Rendering;

namespace Grigor.Gameplay.Lighting
{
    public class LevelLighting : MonoBehaviour
    {
        [SerializeField] private SunController sunController;
        [SerializeField] private WeatherController weatherController;
        [SerializeField] private Volume skyAndFogVolume;
        [SerializeField] private Volume postProcessingVolume;

        public SunController SunController => sunController;
        public WeatherController WeatherController => weatherController;
        public Volume SkyAndFogVolume => skyAndFogVolume;
        public Volume PostProcessingVolume => postProcessingVolume;
    }
}
