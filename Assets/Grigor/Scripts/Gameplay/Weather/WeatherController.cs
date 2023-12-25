using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data;
using Grigor.Gameplay.Lighting;
using Grigor.Gameplay.Time;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Grigor.Gameplay.Weather
{
    [Injectable]
    public class WeatherController : MonoBehaviour
    {
        [SerializeField, ColoredBoxGroup("References", false, true)] private Volume skyAndFogVolume;
        [SerializeField, ColoredBoxGroup("References")] private Volume postProcessingVolume;
        [SerializeField, ColoredBoxGroup("References")] private Material sharedRainMaterial;
        [SerializeField, ColoredBoxGroup("Sky and Fog Components", false, true), ReadOnly] private Fog fog;
        [SerializeField, ColoredBoxGroup("Sky and Fog Components"), ReadOnly] private VolumetricClouds volumetricClouds;
        [SerializeField, ColoredBoxGroup("Post-Processing Components", false, true), ReadOnly] private Exposure exposure;

        [Inject] private TimeManager timeManager;
        [Inject] private RainZoneManager rainZoneManager;
        [Inject] private LightingController lightingController;

        [ShowInInspector, ReadOnly, ColoredBoxGroup("Debug", true, true), HideLabel] private PermanentWeatherData currentPermanentWeatherData;

        private PermanentWeatherData initialPermanentWeatherData;
        private SceneConfig sceneConfig;

        private static readonly int RainStrength = Shader.PropertyToID("_RainStrength");
        private static readonly int DropSpeed = Shader.PropertyToID("_DropSpeed");
        private static readonly int Wetness = Shader.PropertyToID("_Wetness");
        private static readonly int WindStrength = Shader.PropertyToID("_WindStrength");
        private static readonly int WindSpeed = Shader.PropertyToID("_WindSpeed");
        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

        public void Initialize()
        {
            Injector.Inject(this);

            GetVolumeComponents();

            SaveInitialPermanentData();

            sceneConfig = SceneConfig.Instance;

            timeManager.TimeChangedEvent += OnTimeChanged;
        }

        public void Dispose()
        {
            LoadInitialPermanentData();

            timeManager.TimeChangedEvent -= OnTimeChanged;

            Injector.Release(this);
        }

        // NOTE: changes to permanent data, such as the volume assets and the shared rain material we edit,
        // are saved even if changed during runtime. because this happens always, we need to avoid source control issues
        // and reload the initial data from the assets when the scene is loaded to not constantly make changes to these assets.
        // this also enables whoever is working on the scene to create their own initial data and not have the weather cycle
        // affect their work.
        private void SaveInitialPermanentData()
        {
            initialPermanentWeatherData.SetFogAttenuationDistance(fog.meanFreePath.value);
            initialPermanentWeatherData.SetCloudDensity(volumetricClouds.densityMultiplier.value);
            initialPermanentWeatherData.SetRainStrength(sharedRainMaterial.GetFloat(RainStrength));
            initialPermanentWeatherData.SetRainDropSpeed(sharedRainMaterial.GetFloat(DropSpeed));
            initialPermanentWeatherData.SetWetness(sharedRainMaterial.GetFloat(Wetness));
            initialPermanentWeatherData.SetWindStrength(sharedRainMaterial.GetFloat(WindStrength));
            initialPermanentWeatherData.SetWindSpeed(sharedRainMaterial.GetFloat(WindSpeed));
            initialPermanentWeatherData.SetGroundSmoothness(sharedRainMaterial.GetFloat(Smoothness));

            initialPermanentWeatherData.SetCloudShapeFactor(volumetricClouds.shapeFactor.value);
            initialPermanentWeatherData.SetCloudErosionFactor(volumetricClouds.erosionFactor.value);
            initialPermanentWeatherData.SetCloudMicroErosionFactor(volumetricClouds.microErosionFactor.value);

            initialPermanentWeatherData.SetExposure(exposure.fixedExposure.value);
        }

        private void LoadInitialPermanentData()
        {
            SetFog(initialPermanentWeatherData.FogAttenuationDistance);
            SetCloudDensity(initialPermanentWeatherData.CloudDensity);
            SetRainStrength(initialPermanentWeatherData.RainStrength);
            SetGroundRainDropSpeed(initialPermanentWeatherData.RainDropSpeed);
            SetWetness(initialPermanentWeatherData.Wetness);
            SetPuddleWindStrength(initialPermanentWeatherData.WindStrength);
            SetPuddleWindSpeed(initialPermanentWeatherData.WindSpeed);
            SetGroundSmoothness(initialPermanentWeatherData.GroundSmoothness);

            SetCloudShapeFactor(initialPermanentWeatherData.CloudShapeFactor);
            SetCloudErosionFactor(initialPermanentWeatherData.CloudErosionFactor);
            SetCloudMicroErosionFactor(initialPermanentWeatherData.CloudMicroErosionFactor);

            SetExposure(initialPermanentWeatherData.Exposure);
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
            SetCurrentEvaluatedWeatherDataByDay(1, timeManager.GetCurrentDayPercentage());
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

        private void SetPuddleWindStrength(float value)
        {
            sharedRainMaterial.SetFloat(WindStrength, value);
        }

        private void SetPuddleWindSpeed(float value)
        {
            sharedRainMaterial.SetFloat(WindSpeed, value);
        }

        private void SetGroundSmoothness(float value)
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

        private void SetCurrentEvaluatedWeatherDataByDay(int day, float percentage)
        {
            int index = day - 1;

            if (index < 0 || index >= sceneConfig.WeatherDataList.Count)
            {
                throw Log.Exception($"Day {day} is out of bounds of supported days for the weather!");
            }

            float cloudDensity = sceneConfig.WeatherDataList[index].EvaluateCloudDensity(percentage);
            float windStrength = sceneConfig.WeatherDataList[index].EvaluateWindStrength(percentage);
            float fogStrength = sceneConfig.WeatherDataList[index].EvaluateFogStrength(percentage);
            float cloudShapeFactor = sceneConfig.WeatherDataList[index].EvaluateCloudShapeFactor(percentage);
            float cloudErosionFactor = sceneConfig.WeatherDataList[index].EvaluateCloudErosionFactor(percentage);
            float cloudMicroErosionFactor = sceneConfig.WeatherDataList[index].EvaluateCloudMicroErosionFactor(percentage);

            float newExposure = Mathf.Lerp(sceneConfig.NighttimeExposure, sceneConfig.DaytimeExposure, CustomExposureRemap(lightingController.CurrentSunRotation, sceneConfig.ExposureMinimumSunRotationAngle, sceneConfig.ExposureMaximumSunRotationAngle));

            float rainStrength = CalculateRainStrength(cloudDensity, cloudShapeFactor);

            SetExposure(newExposure);

            SetFog(Mathf.Lerp(sceneConfig.FogAttenuationDistanceBounds .x, sceneConfig.FogAttenuationDistanceBounds.y, 1 - fogStrength));
            SetRainStrength(rainStrength);
            SetGroundRainDropSpeed(Mathf.Lerp(sceneConfig.RainDropSpeedBounds.x, sceneConfig.RainDropSpeedBounds.y, rainStrength));
            SetWetness(Mathf.Lerp(sceneConfig.SmoothnessBounds.x, sceneConfig.SmoothnessBounds.y, rainStrength));
            SetPuddleWindStrength(windStrength);
            SetPuddleWindSpeed(Mathf.Lerp(sceneConfig.WindSpeedBounds.x, sceneConfig.WindSpeedBounds.y, windStrength));
            SetGroundSmoothness(Mathf.Lerp(sceneConfig.SmoothnessBounds.x, sceneConfig.SmoothnessBounds.y, rainStrength));

            SetCloudDensity(cloudDensity);
            SetCloudShapeFactor(Mathf.Lerp(sceneConfig.CloudShapeFactorMinimum, 1, cloudShapeFactor));
            SetCloudErosionFactor(cloudErosionFactor);
            SetCloudMicroErosionFactor(cloudMicroErosionFactor);

            rainZoneManager.SetRainParticleEmission(Mathf.Lerp(sceneConfig.RainParticleEmissionBounds.x, sceneConfig.RainParticleEmissionBounds.y, rainStrength));
        }

        private float CalculateRainStrength(float cloudDensity, float cloudShapeFactor)
        {
            if (cloudDensity < sceneConfig.CloudDensityRainMinimumThreshold)
            {
                return 0;
            }

            if (cloudShapeFactor > sceneConfig.CloudShapeFactorRainMaximumThreshold)
            {
                return 0;
            }

            float rainStrengthFromCloudDensity = (cloudDensity - sceneConfig.CloudDensityRainMinimumThreshold) * 2;
            float rainStrengthFromCloudShapeFactor = 1 - (sceneConfig.CloudShapeFactorRainMaximumThreshold - cloudShapeFactor);

            float rainStrength = rainStrengthFromCloudDensity * 0.7f + rainStrengthFromCloudShapeFactor * 0.3f;

            return Mathf.Clamp01(rainStrength);
        }

        /// <summary>
        /// If the sun angle is between the minAngle and the maxAngle, the exposure will be either the daytime or nighttime exposure (depending on the sign). If it outside of this range, it will transition smoothly between the two.
        /// </summary>
        private float CustomExposureRemap(float value, float minAngle, float maxAngle)
        {
            float remappedAngle = (value + 360f) % 360f;
            float exposureRemap;

            //remapping the angle to be between -180 and 180
            if (remappedAngle is > 180f or < -180f)
            {
                remappedAngle -= 360f;
            }

            float absAngle = Mathf.Abs(remappedAngle);

            //if the angle is between the min and max angle, the exposure will be either the daytime or nighttime exposure (depending on the sign)
            if (absAngle >= minAngle && absAngle <= maxAngle)
            {
                exposureRemap = remappedAngle < 0f ? 0 : 1;

                return exposureRemap;
            }

            float positiveAngle = remappedAngle < 0f ? 360f + remappedAngle : remappedAngle;

            //if the sun is setting, the value goes from 1 to 0, and if it is rising, it goes from 0 to 1
            exposureRemap = positiveAngle is > 90f and < 270f ? Mathf.InverseLerp(maxAngle + 90f, maxAngle, positiveAngle) : Mathf.InverseLerp(-minAngle, minAngle, remappedAngle);

            return Mathf.Clamp01(exposureRemap);
        }
    }
}
