using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Gameplay.Weather;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Grigor.Data
{
    [GlobalConfig, CreateAssetMenu(fileName = "WeatherConfig", menuName = "Grigor/Weather Config")]
    public class WeatherConfig : GlobalConfig<WeatherConfig>
    {
        [InfoBox("These controls affect the value of the property from 00:00 to 23:59.")]
        [SerializeField] private List<DailyWeatherData> weatherDataList;

        [PropertyTooltip("The minimum cloud density for rain to start.")]
        [SerializeField, Range(0f, 1f)] private float cloudDensityRainThreshold = 0.5f;

        [PropertyTooltip("The wind speed range for volumetric clouds.")]
        [SerializeField] private Vector2 windSpeedBounds;

        [PropertyTooltip("The fog attenuation distance range.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals", false, true)] private Vector2 fogAttenuationDistanceBounds;

        [PropertyTooltip("The rain drop speed range for drops on the ground.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private Vector2 rainDropSpeedBounds;

        [PropertyTooltip("The wind speed range for puddles that form on the ground.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private Vector2 puddleWindSpeedBounds;

        [PropertyTooltip("How many minutes does it take for the ground to become completely dry?")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private float minutesUntilNoLongerWet;

        [PropertyTooltip("The smoothness range for the wetness of the ground.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private Vector2 smoothnessBounds;

        public EvaluatedWeatherData GetCurrentEvaluatedWeatherDataByDay(int day, float percentage)
        {
            int index = day - 1;

            if (index < 0 || index >= weatherDataList.Count)
            {
                throw Log.Exception($"Day {day} is out of bounds of supported days for the weather!");
            }

            float cloudDensity = weatherDataList[index].EvaluateCloudDensity(percentage);
            float windStrength = weatherDataList[index].EvaluateWindStrength((int)percentage);
            float fogStrength = weatherDataList[index].EvaluateFogStrength((int)percentage);

            float rainStrength = cloudDensity > cloudDensityRainThreshold ? Mathf.Clamp01((cloudDensity - cloudDensityRainThreshold) / (1 - cloudDensityRainThreshold)) : 0f;

            EvaluatedWeatherData evaluatedWeatherData = new EvaluatedWeatherData();

            evaluatedWeatherData.SetCloudDensity(cloudDensity);
            evaluatedWeatherData.SetWindSpeed(Mathf.Lerp(windSpeedBounds.x, windSpeedBounds.y, windStrength));
            evaluatedWeatherData.SetWindStrength(windStrength);
            evaluatedWeatherData.SetFogAttenuationDistance(Mathf.Lerp(fogAttenuationDistanceBounds .x, fogAttenuationDistanceBounds.y, 1 - fogStrength));
            evaluatedWeatherData.SetPuddleWindSpeed(Mathf.Lerp(puddleWindSpeedBounds.x, puddleWindSpeedBounds.y, windStrength));
            evaluatedWeatherData.SetRainDropSpeed(Mathf.Lerp(rainDropSpeedBounds.x, rainDropSpeedBounds.y, rainStrength));
            evaluatedWeatherData.SetRainStrength(rainStrength);

            evaluatedWeatherData.SetWetness(Mathf.Lerp(smoothnessBounds.x, smoothnessBounds.y, rainStrength));
            evaluatedWeatherData.SetGroundSmoothness(Mathf.Lerp(smoothnessBounds.x, smoothnessBounds.y, rainStrength));

            return evaluatedWeatherData;
        }
    }
}
