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

        private EvaluatedWeatherData initialWeatherData = new();
        private SceneConfig sceneConfig;

        private static readonly int RainStrength = Shader.PropertyToID("_RainStrength");
        private static readonly int DropSpeed = Shader.PropertyToID("_DropSpeed");
        private static readonly int Wetness = Shader.PropertyToID("_Wetness");
        private static readonly int WindStrength = Shader.PropertyToID("_WindStrength");
        private static readonly int WindSpeed = Shader.PropertyToID("_WindSpeed");
        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

        protected override void OnInjected()
        {
            GetVolumeComponents();

            SaveInitialPermanentData();

            timeManager.TimeChangedEvent += OnTimeChanged;
        }

        protected override void OnReleased()
        {
            LoadInitialPermanentData();

            timeManager.TimeChangedEvent -= OnTimeChanged;
        }

        // NOTE: changes to permanent data, such as the volume assets and the shared rain material we edit,
        // are saved even if changed during runtime. because this happens always, we need to avoid source control issues
        // and reload the initial data from the assets when the scene is loaded to not constantly make changes to these assets.
        // this also enables whoever is working on the scene to create their own initial data and not have the weather cycle
        // affect their work.
        private void SaveInitialPermanentData()
        {
            initialWeatherData.SetFogAttenuationDistance(fog.meanFreePath.value);
            initialWeatherData.SetCloudDensity(volumetricClouds.densityMultiplier.value);
            initialWeatherData.SetRainStrength(sharedRainMaterial.GetFloat(RainStrength));
            initialWeatherData.SetRainDropSpeed(sharedRainMaterial.GetFloat(DropSpeed));
            initialWeatherData.SetWetness(sharedRainMaterial.GetFloat(Wetness));
            initialWeatherData.SetWindStrength(sharedRainMaterial.GetFloat(WindStrength));
            initialWeatherData.SetWindSpeed(sharedRainMaterial.GetFloat(WindSpeed));
            initialWeatherData.SetGroundSmoothness(sharedRainMaterial.GetFloat(Smoothness));

            initialWeatherData.SetCloudShapeFactor(volumetricClouds.shapeFactor.value);
            initialWeatherData.SetCloudErosionFactor(volumetricClouds.erosionFactor.value);
            initialWeatherData.SetCloudMicroErosionFactor(volumetricClouds.microErosionFactor.value);

            initialWeatherData.SetExposure(exposure.fixedExposure.value);
        }

        private void LoadInitialPermanentData()
        {
            SetFog(initialWeatherData.FogAttenuationDistance);
            SetCloudDensity(initialWeatherData.CloudDensity);
            SetRainStrength(initialWeatherData.RainStrength);
            SetGroundRainDropSpeed(initialWeatherData.RainDropSpeed);
            SetWetness(initialWeatherData.Wetness);
            SetWindStrength(initialWeatherData.WindStrength);
            SetWindSpeed(initialWeatherData.WindSpeed);
            SetSmoothness(initialWeatherData.GroundSmoothness);

            SetCloudShapeFactor(initialWeatherData.CloudShapeFactor);
            SetCloudErosionFactor(initialWeatherData.CloudErosionFactor);
            SetCloudMicroErosionFactor(initialWeatherData.CloudMicroErosionFactor);

            SetExposure(initialWeatherData.Exposure);
        }

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
