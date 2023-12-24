using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Gameplay.Weather;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Grigor.Data
{
    [GlobalConfig, CreateAssetMenu(fileName = "SceneConfig", menuName = "Grigor/Scene Config")]
    public class SceneConfig : GlobalConfig<SceneConfig>
    {
        [SerializeField, ColoredBoxGroup("Lighting", true, 0.2f, 0.5f, 0.5f)] private Gradient ambientColor;
        [SerializeField, ColoredBoxGroup("Lighting")] private Gradient directionalColor;
        [SerializeField, ColoredBoxGroup("Lighting")] private Gradient fogColor;
        [SerializeField, ColoredBoxGroup("Lighting") ,Range(0, 14f)] private float daytimeExposure;
        [SerializeField, ColoredBoxGroup("Lighting"), Range(0, 14f)] private float nighttimeExposure;

        [PropertyTooltip("An angle smaller than 90 degrees and greater than 0 that represents the first angle of the sun's rotation in the sky when the exposure is the brightest. Also works vice-versa for nighttime.")]
        [SerializeField, ColoredBoxGroup("Lighting"), Range(1, 89)] private int exposureMinimumSunRotationAngle;

        [PropertyTooltip("An angle greater than 90 degrees and smaller than 180 that represents the last angle of the sun's rotation in the sky when the exposure is the brightest. Also works vice-versa for nighttime.")]
        [SerializeField, ColoredBoxGroup("Lighting"), Range(91, 179)] private int exposureMaximumSunRotationAngle;

        [SerializeField, ColoredBoxGroup("Time", false, true), Range(0, 24)] private int dayStartHour = 8;
        [SerializeField, ColoredBoxGroup("Time"), Range(0, 24)] private int nightStartHour = 22;
        [SerializeField, ColoredBoxGroup("Time"), Range(0, 24)] private int startHour = 8;
        [SerializeField, ColoredBoxGroup("Time")] private bool changeTimeAutomatically;
        [SerializeField, ColoredBoxGroup("Time"), ShowIf(nameof(changeTimeAutomatically))] private float timeMultiplier;

        [InfoBox("These controls affect the value of the property from 00:00 to 23:59.")]
        [SerializeField] private List<DailyWeatherData> weatherDataList;

        [PropertyTooltip("The minimum cloud density for rain to start.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky", false, true), Range(0f, 1f)] private float cloudDensityRainMinimumThreshold = 0.5f;

        [PropertyTooltip("The smallest the cloud shape factor can be.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky"), Range(0f, 1f)] private float cloudShapeFactorMinimum;

        [PropertyTooltip("The maximum value the cloud shape factor can reach before rain stops.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky"), Range(0f, 1f)] private float cloudShapeFactorRainMaximumThreshold;

        [PropertyTooltip("The slowest and fastest wind speeds possible.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky")] private Vector2 windSpeedBounds;

        [PropertyTooltip("The least and most rain particle emission possible.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky")] private Vector2 rainParticleEmissionBounds;

        [PropertyTooltip("The smallest and largest possible cloud shape scale, chosen randomly between the bounds to be tweened to every hour.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky")] private Vector2 cloudShapeScaleBounds;

        [PropertyTooltip("The smallest and largest possible cloud erosion scale, chosen randomly between the bounds to be tweened to every hour.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky")] private Vector2 cloudErosionScaleBounds;

        [PropertyTooltip("The strongest and weakest fog values possible.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals", false, true)] private Vector2 fogAttenuationDistanceBounds;

        [PropertyTooltip("The slowest and fastest speeds at which rain drops spawn and disappear on the ground.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private Vector2 rainDropSpeedBounds;

        [PropertyTooltip("The slowest and fastest wind speeds for the normals puddles on the ground to be affected by.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private Vector2 puddleWindSpeedBounds;

        [PropertyTooltip("How many minutes does it take for the ground to become completely dry?")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private float minutesUntilNoLongerWet;

        [PropertyTooltip("The lowest and highest smoothness values for when the ground is wet.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private Vector2 smoothnessBounds;

        private float currentSunRotation;

        public Gradient AmbientColor => ambientColor;
        public Gradient DirectionalColor => directionalColor;
        public Gradient FogColor => fogColor;

        public int DayStartHour => dayStartHour;
        public int NightStartHour => nightStartHour;
        public int StartHour => startHour;
        public bool ChangeTimeAutomatically => changeTimeAutomatically;
        public float TimeMultiplier => timeMultiplier;

        public EvaluatedWeatherData GetCurrentEvaluatedWeatherDataByDay(int day, float percentage)
        {
            int index = day - 1;

            if (index < 0 || index >= weatherDataList.Count)
            {
                throw Log.Exception($"Day {day} is out of bounds of supported days for the weather!");
            }

            float cloudDensity = weatherDataList[index].EvaluateCloudDensity(percentage);
            float windStrength = weatherDataList[index].EvaluateWindStrength(percentage);
            float fogStrength = weatherDataList[index].EvaluateFogStrength(percentage);
            float cloudShapeFactor = weatherDataList[index].EvaluateCloudShapeFactor(percentage);
            float cloudErosionFactor = weatherDataList[index].EvaluateCloudErosionFactor(percentage);
            float cloudMicroErosionFactor = weatherDataList[index].EvaluateCloudMicroErosionFactor(percentage);

            float exposure = Mathf.Lerp(daytimeExposure, nighttimeExposure, CustomExposureRemap(currentSunRotation, exposureMinimumSunRotationAngle, exposureMaximumSunRotationAngle));

            float rainStrength = cloudDensity > cloudDensityRainMinimumThreshold ? Mathf.Clamp01((cloudDensity - cloudDensityRainMinimumThreshold) / (1 - cloudDensityRainMinimumThreshold)) : 0f;

            EvaluatedWeatherData evaluatedWeatherData = new EvaluatedWeatherData();

            evaluatedWeatherData.SetCloudDensity(cloudDensity);
            evaluatedWeatherData.SetWindSpeed(Mathf.Lerp(windSpeedBounds.x, windSpeedBounds.y, windStrength));
            evaluatedWeatherData.SetRainParticleEmission(Mathf.Lerp(rainParticleEmissionBounds.x, rainParticleEmissionBounds.y, rainStrength));
            evaluatedWeatherData.SetWindStrength(windStrength);
            evaluatedWeatherData.SetFogAttenuationDistance(Mathf.Lerp(fogAttenuationDistanceBounds .x, fogAttenuationDistanceBounds.y, 1 - fogStrength));
            evaluatedWeatherData.SetPuddleWindSpeed(Mathf.Lerp(puddleWindSpeedBounds.x, puddleWindSpeedBounds.y, windStrength));
            evaluatedWeatherData.SetRainDropSpeed(Mathf.Lerp(rainDropSpeedBounds.x, rainDropSpeedBounds.y, rainStrength));
            evaluatedWeatherData.SetRainStrength(rainStrength);

            evaluatedWeatherData.SetCloudShapeFactor(Mathf.Lerp(cloudShapeFactorMinimum, 1, cloudShapeFactor));
            evaluatedWeatherData.SetCloudErosionFactor(cloudErosionFactor);
            evaluatedWeatherData.SetCloudMicroErosionFactor(cloudMicroErosionFactor);

            evaluatedWeatherData.SetWetness(Mathf.Lerp(smoothnessBounds.x, smoothnessBounds.y, rainStrength));
            evaluatedWeatherData.SetGroundSmoothness(Mathf.Lerp(smoothnessBounds.x, smoothnessBounds.y, rainStrength));

            evaluatedWeatherData.SetExposure(exposure);

            return evaluatedWeatherData;
        }

        public void OnSunRotationUpdated(float rotation)
        {
            currentSunRotation = rotation;
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
