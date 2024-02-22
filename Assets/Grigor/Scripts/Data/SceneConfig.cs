using System.Collections.Generic;
using Grigor.Gameplay.Settings;
using Grigor.Gameplay.Weather;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Grigor.Data
{
    [GlobalConfig("Assets/Resources/Config/"), CreateAssetMenu(fileName = "SceneConfig", menuName = "Grigor/Scene Config")]
    public class SceneConfig : GlobalConfig<SceneConfig>
    {
        [SerializeField, ColoredBoxGroup("Settings", false, true)] private bool rainEnabled = true;
        [SerializeField, ColoredBoxGroup("Settings", false, true)] private bool volumetricsEnabled = true;
        [SerializeField, ColoredBoxGroup("Settings")] private List<Vector2> resolutionOptions;
        [SerializeField, ColoredBoxGroup("Settings")] private List<QualityOptions> qualityOptions;

        [SerializeField, ColoredBoxGroup("Lighting", true, 0.2f, 0.5f, 0.5f)] private Gradient ambientColor;
        [SerializeField, ColoredBoxGroup("Lighting")] private Gradient directionalColor;
        [SerializeField, ColoredBoxGroup("Lighting") ,Range(0, 14f)] private float daytimeExposure;
        [SerializeField, ColoredBoxGroup("Lighting"), Range(0, 14f)] private float nighttimeExposure;
        [SerializeField, ColoredBoxGroup("Lighting")] private bool smoothSunTransition;
        [SerializeField, ColoredBoxGroup("Lighting"), ShowIf(nameof(smoothSunTransition))] private float smoothSunTransitionTime;

        [Tooltip("An angle smaller than 90 degrees and greater than 0 that represents the first angle of the sun's rotation in the sky when the exposure is the brightest. Also works vice-versa for nighttime.")]
        [SerializeField, ColoredBoxGroup("Lighting"), Range(1, 89)] private int exposureMinimumSunRotationAngle;

        [Tooltip("An angle greater than 90 degrees and smaller than 180 that represents the last angle of the sun's rotation in the sky when the exposure is the brightest. Also works vice-versa for nighttime.")]
        [SerializeField, ColoredBoxGroup("Lighting"), Range(91, 179)] private int exposureMaximumSunRotationAngle;

        [SerializeField, ColoredBoxGroup("Time", false, true), Range(0, 24)] private int dayStartHour = 8;
        [SerializeField, ColoredBoxGroup("Time"), Range(0, 24)] private int nightStartHour = 22;
        [SerializeField, ColoredBoxGroup("Time"), Range(0, 24)] private int startHour = 8;
        [SerializeField, ColoredBoxGroup("Time")] private bool changeTimeAutomatically;
        [SerializeField, ColoredBoxGroup("Time"), ShowIf(nameof(changeTimeAutomatically)), Range(1, 15000)] private float timeMultiplier;

        [InfoBox("These controls affect the value of the property from 00:00 to 23:59.")]
        [SerializeField, ColoredBoxGroup("Daily Weather", false, true)] private List<DailyWeatherData> weatherDataList;
        [SerializeField, ColoredBoxGroup("Daily Weather")] private bool repeatFirstDayWeather;

        [Tooltip("The smallest the cloud shape factor can be.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky", false, true), Range(0f, 1f)] private float cloudShapeFactorMinimum;

        [Tooltip("The slowest and fastest wind speeds possible.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky")] private Vector2 windSpeedBounds;

        [Tooltip("The least and most rain particle emission possible.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky")] private Vector2 rainParticleEmissionBounds;

        [Tooltip("The smallest and largest possible cloud shape scale, chosen randomly between the bounds to be tweened to every hour.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky")] private Vector2 cloudShapeScaleBounds;

        [Tooltip("The smallest and largest possible cloud erosion scale, chosen randomly between the bounds to be tweened to every hour.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky")] private Vector2 cloudErosionScaleBounds;

        [Tooltip("The largest possible angle at which the rain can tilt according to the wind direction.")]
        [SerializeField, ColoredBoxGroup("Rain and Sky"), Range(0f, 90f)] private float maximumRainTiltAngle;

        [Tooltip("The strongest and weakest fog values possible.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals", false, true)] private Vector2 fogAttenuationDistanceBounds;

        [Tooltip("The slowest and fastest speeds at which rain drops spawn and disappear on the ground.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private Vector2 rainDropSpeedBounds;

        [Tooltip("The slowest and fastest wind speeds for the normals puddles on the ground to be affected by.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private Vector2 puddleWindSpeedBounds;

        [Tooltip("How many minutes does it take for the ground to become completely dry?")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private float minutesUntilFullyDry;

        [Tooltip("How many minutes does it take for the ground to become completely dry?")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private float minutesUntilFullyWet;

        [Tooltip("The lowest and highest smoothness values for when the ground is wet.")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals")] private Vector2 smoothnessBounds;

        [Tooltip("How much does rain strength affect the speed of how fast the ground gets wet?")]
        [SerializeField, ColoredBoxGroup("Wet Ground Visuals"), Range(0f, 1f)] private float rainStrengthWetnessFactor;

        [Tooltip("The amount of time in minutes until a new random number is chosen for the values below.")]
        [SerializeField, ColoredBoxGroup("Random Weather Changes", false, true), Range(0f, 1440f)] private float minutesInBetweenWeatherChanges;

        [Tooltip("The chance that the cloud shape scale will change when the weather changes.")]
        [SerializeField, ColoredBoxGroup("Random Weather Changes"), Range(0f, 1f)] private float cloudShapeScaleChangeChance;

        [Tooltip("The chance that the cloud erosion scale will change when the weather changes.")]
        [SerializeField, ColoredBoxGroup("Random Weather Changes"), Range(0f, 1f)] private float cloudErosionScaleChangeChance;

        [Tooltip("The chance that the wind speed will change when the weather changes.")]
        [SerializeField, ColoredBoxGroup("Random Weather Changes"), Range(0f, 1f)] private float windDirectionChangeChance;

        public bool RainEnabled => rainEnabled;
        public bool VolumetricsEnabled => volumetricsEnabled;
        public List<Vector2> ResolutionOptions => resolutionOptions;
        public List<QualityOptions> QualityOptions => qualityOptions;

        public Gradient AmbientColor => ambientColor;
        public Gradient DirectionalColor => directionalColor;
        public float DaytimeExposure => daytimeExposure;
        public float NighttimeExposure => nighttimeExposure;
        public int ExposureMinimumSunRotationAngle => exposureMinimumSunRotationAngle;
        public int ExposureMaximumSunRotationAngle => exposureMaximumSunRotationAngle;

        public int DayStartHour => dayStartHour;
        public int NightStartHour => nightStartHour;
        public int StartHour => startHour;
        public bool ChangeTimeAutomatically => changeTimeAutomatically;
        public float TimeMultiplier => timeMultiplier;

        public List<DailyWeatherData> WeatherDataList => weatherDataList;
        public bool RepeatFirstDayWeather => repeatFirstDayWeather;
        public float CloudShapeFactorMinimum => cloudShapeFactorMinimum;
        public Vector2 WindSpeedBounds => windSpeedBounds;
        public Vector2 RainParticleEmissionBounds => rainParticleEmissionBounds;
        public Vector2 CloudShapeScaleBounds => cloudShapeScaleBounds;
        public Vector2 CloudErosionScaleBounds => cloudErosionScaleBounds;
        public Vector2 FogAttenuationDistanceBounds => fogAttenuationDistanceBounds;
        public Vector2 RainDropSpeedBounds => rainDropSpeedBounds;
        public Vector2 PuddleWindSpeedBounds => puddleWindSpeedBounds;
        public float MinutesUntilFullyDry => minutesUntilFullyDry;
        public Vector2 SmoothnessBounds => smoothnessBounds;
        public float MinutesUntilFullyWet => minutesUntilFullyWet;
        public float RainStrengthWetnessFactor => rainStrengthWetnessFactor;
        public float MaximumRainTiltAngle => maximumRainTiltAngle;

        public bool SmoothSunTransition => smoothSunTransition;
        public float SmoothSunTransitionTime => smoothSunTransitionTime;

        public float MinutesInBetweenWeatherChanges => minutesInBetweenWeatherChanges;
        public float CloudShapeScaleChangeChance => cloudShapeScaleChangeChance;
        public float CloudErosionScaleChangeChance => cloudErosionScaleChangeChance;
        public float WindDirectionChangeChance => windDirectionChangeChance;

        public void EnableRain()
        {
            rainEnabled = true;
        }

        public void DisableRain()
        {
            rainEnabled = false;
        }

        public void EnableVolumetrics()
        {
            volumetricsEnabled = true;
        }

        public void DisableVolumetrics()
        {
            volumetricsEnabled = false;
        }
    }
}
