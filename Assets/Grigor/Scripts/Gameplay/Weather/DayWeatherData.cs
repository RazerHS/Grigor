using System;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Gameplay.Weather
{
    [Serializable]
    public class DailyWeatherData
    {
        [SerializeField, ColoredBoxGroup("Weather", false, true)] private AnimationCurve cloudDensityCurve;
        [SerializeField, ColoredBoxGroup("Weather")] private AnimationCurve windStrength;
        [SerializeField, ColoredBoxGroup("Weather")] private AnimationCurve fogStrength;

        public float EvaluateCloudDensity(float percentage)
        {
            return Mathf.Clamp01(cloudDensityCurve.Evaluate(percentage));
        }

        public float EvaluateWindStrength(int percentage)
        {
            return Mathf.Clamp01(windStrength.Evaluate(percentage));
        }

        public float EvaluateFogStrength(int percentage)
        {
            return Mathf.Clamp01(fogStrength.Evaluate(percentage));
        }
    }
}
