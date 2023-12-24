using System;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Weather
{
    [Serializable]
    public class DailyWeatherData
    {
        [PropertyTooltip("Visually shows what the curves below will evaluate to at the selected time of day as a reference when designing the weather from 00:00 to 23:59.")]
        [ShowInInspector, ColoredBoxGroup("Weather", false, true), ProgressBar(0, 24, Segmented = false, Height = 15)] private int timeReference = 12;

        [PropertyTooltip("Evaluates to how dense the clouds are at any given time. A higher cloud density makes the clouds darker and more opaque because they have absorbed more water, meaning a higher density is used when it is about to rain.")]
        [SerializeField, ColoredBoxGroup("Weather", false, true)] private AnimationCurve cloudDensityCurve;

        [PropertyTooltip("Evaluates to the strength of the wind at any given time. A higher wind strength means the clouds will move faster and puddles on the ground will have faster moving normals.")]
        [SerializeField, ColoredBoxGroup("Weather")] private AnimationCurve windStrength;

        [PropertyTooltip("Evaluates to the strength of the fog at any given time. A higher fog strength means the fog will be more opaque and the sky will be less visible.")]
        [SerializeField, ColoredBoxGroup("Weather")] private AnimationCurve fogStrength;

        [PropertyTooltip("Evaluates to the shape of the clouds at any given time. A higher cloud shape factor means the clouds will be more spread out and less dense.")]
        [SerializeField, ColoredBoxGroup("Weather")] private AnimationCurve cloudShapeFactor;

        [PropertyTooltip("Evaluates to the erosion of the clouds at any given time. A higher cloud erosion factor will erode the edges of clouds more significantly, and lower values will make clouds smoother.")]
        [SerializeField, ColoredBoxGroup("Weather")] private AnimationCurve cloudErosionFactor;

        [PropertyTooltip(" Evaluates to the micro erosion of the clouds at any given time. Very similar to the cloud erosion factor, but this one is more subtle and is used to make the clouds look more realistic.")]
        [SerializeField, ColoredBoxGroup("Weather")] private AnimationCurve cloudMicroErosionFactor;

        public float EvaluateCloudDensity(float percentage)
        {
            return Mathf.Clamp01(cloudDensityCurve.Evaluate(percentage));
        }

        public float EvaluateWindStrength(float percentage)
        {
            return Mathf.Clamp01(windStrength.Evaluate(percentage));
        }

        public float EvaluateFogStrength(float percentage)
        {
            return Mathf.Clamp01(fogStrength.Evaluate(percentage));
        }

        public float EvaluateCloudShapeFactor(float percentage)
        {
            return Mathf.Clamp01(cloudShapeFactor.Evaluate(percentage));
        }

        public float EvaluateCloudErosionFactor(float percentage)
        {
            return Mathf.Clamp01(cloudErosionFactor.Evaluate(percentage));
        }

        public float EvaluateCloudMicroErosionFactor(float percentage)
        {
            return Mathf.Clamp01(cloudMicroErosionFactor.Evaluate(percentage));
        }
    }
}
