using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data;
using Grigor.Gameplay.Time;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Grigor.Gameplay.Weather
{
    public class WeatherController : CardboardCoreBehaviour
    {
        [SerializeField, ColoredBoxGroup("References", false, true)] private Volume skyAndFogVolume;
        [SerializeField, ColoredBoxGroup("References")] private Volume postProcessingVolume;
        [SerializeField, ColoredBoxGroup("References")] private Material sharedRainMaterial;
        [SerializeField, ColoredBoxGroup("Sky and Fog Components", false, true), ReadOnly] private Fog fog;
        [SerializeField, ColoredBoxGroup("Sky and Fog Components"), ReadOnly] private VolumetricClouds volumetricClouds;
        [SerializeField, ColoredBoxGroup("Post-Processing Components", false, true), ReadOnly] private Exposure exposure;

        [Inject] private TimeManager timeManager;
        [Inject] private RainZoneManager rainZoneManager;

        [ShowInInspector, ReadOnly] private EvaluatedWeatherData currentEvaluatedWeatherData;

        private static readonly int RainStrength = Shader.PropertyToID("_RainStrength");
        private static readonly int DropSpeed = Shader.PropertyToID("_DropSpeed");
        private static readonly int Wetness = Shader.PropertyToID("_Wetness");
        private static readonly int WindStrength = Shader.PropertyToID("_WindStrength");
        private static readonly int WindSpeed = Shader.PropertyToID("_WindSpeed");
        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

        [Button(ButtonSizes.Large)]
        private void GetVolumeComponents()
        {
            if (!skyAndFogVolume.sharedProfile.TryGet(out fog))
            {
                throw Log.Exception("Fog component not found in volume!");
            }

            if (!skyAndFogVolume.sharedProfile.TryGet(out volumetricClouds))
            {
                throw Log.Exception("Volumetric Clouds component not found in volume!");
            }

            if (!postProcessingVolume.sharedProfile.TryGet(out exposure))
            {
                throw Log.Exception("Exposure component not found in volume!");
            }
        }

        protected override void OnInjected()
        {
            timeManager.TimeChangedEvent += OnTimeChanged;
        }

        protected override void OnReleased()
        {
            timeManager.TimeChangedEvent -= OnTimeChanged;
        }

        private void OnTimeChanged(int minutes, int hours)
        {
            currentEvaluatedWeatherData = SceneConfig.Instance.GetCurrentEvaluatedWeatherDataByDay(1, timeManager.GetCurrentDayPercentage());

            SetFog(currentEvaluatedWeatherData.FogAttenuationDistance);
            SetCloudDensity(currentEvaluatedWeatherData.CloudDensity);
            SetRainStrength(currentEvaluatedWeatherData.RainStrength);
            SetGroundRainDropSpeed(currentEvaluatedWeatherData.RainDropSpeed);
            SetWetness(currentEvaluatedWeatherData.Wetness);
            SetWindStrength(currentEvaluatedWeatherData.WindStrength);
            SetWindSpeed(currentEvaluatedWeatherData.WindSpeed);
            SetSmoothness(currentEvaluatedWeatherData.GroundSmoothness);

            SetCloudShapeFactor(currentEvaluatedWeatherData.CloudShapeFactor);
            SetCloudErosionFactor(currentEvaluatedWeatherData.CloudErosionFactor);
            SetCloudMicroErosionFactor(currentEvaluatedWeatherData.CloudMicroErosionFactor);

            SetExposure(currentEvaluatedWeatherData.Exposure);

            rainZoneManager.SetRainStrength(currentEvaluatedWeatherData.RainParticleEmission);
        }

        private void SetFog(float value)
        {
            fog.meanFreePath.value = value;
        }

        private void SetCloudDensity(float value)
        {
            volumetricClouds.densityMultiplier.value = value;
        }

        private void SetRainStrength(float value)
        {
            sharedRainMaterial.SetFloat(RainStrength, value);
        }

        private void SetGroundRainDropSpeed(float value)
        {
            sharedRainMaterial.SetFloat(DropSpeed, value);
        }

        private void SetWetness(float value)
        {
            sharedRainMaterial.SetFloat(Wetness, value);
        }

        private void SetWindStrength(float value)
        {
            sharedRainMaterial.SetFloat(WindStrength, value);
        }

        private void SetWindSpeed(float value)
        {
            sharedRainMaterial.SetFloat(WindSpeed, value);
        }

        private void SetSmoothness(float value)
        {
            sharedRainMaterial.SetFloat(Smoothness, value);
        }

        private void SetCloudShapeFactor(float value)
        {
            volumetricClouds.shapeFactor.value = value;
        }

        private void SetCloudErosionFactor(float value)
        {
            volumetricClouds.erosionFactor.value = value;
        }

        private void SetCloudMicroErosionFactor(float value)
        {
            volumetricClouds.microErosionFactor.value = value;
        }

        private void SetExposure(float value)
        {
            exposure.fixedExposure.value = value;
        }
    }
}
