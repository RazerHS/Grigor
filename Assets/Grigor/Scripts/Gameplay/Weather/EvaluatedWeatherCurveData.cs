using UnityEngine;

namespace Grigor.Gameplay.Weather
{
    public struct EvaluatedWeatherData
    {
        [SerializeField] private float cloudDensity;
        [SerializeField] private float windSpeed;
        [SerializeField] private float windStrength;
        [SerializeField] private float fogAttenuationDistance;
        [SerializeField] private float puddleWindSpeed;
        [SerializeField] private float rainDropSpeed;
        [SerializeField] private float wetness;
        [SerializeField] private float rainStrength;
        [SerializeField] private float groundSmoothness;

        public float CloudDensity => cloudDensity;
        public float WindSpeed => windSpeed;
        public float WindStrength => windStrength;
        public float FogAttenuationDistance => fogAttenuationDistance;
        public float PuddleWindSpeed => puddleWindSpeed;
        public float RainDropSpeed => rainDropSpeed;
        public float Wetness => wetness;
        public float RainStrength => rainStrength;
        public float GroundSmoothness => groundSmoothness;

        public void SetCloudDensity(float cloudDensity)
        {
            this.cloudDensity = cloudDensity;
        }

        public void SetWindSpeed(float windSpeed)
        {
            this.windSpeed = windSpeed;
        }

        public void SetWindStrength(float windStrength)
        {
            this.windStrength = windStrength;
        }

        public void SetFogAttenuationDistance(float fogAttenuationDistance)
        {
            this.fogAttenuationDistance = fogAttenuationDistance;
        }

        public void SetPuddleWindSpeed(float puddleWindSpeed)
        {
            this.puddleWindSpeed = puddleWindSpeed;
        }

        public void SetRainDropSpeed(float rainDropSpeed)
        {
            this.rainDropSpeed = rainDropSpeed;
        }

        public void SetWetness(float wetness)
        {
            this.wetness = wetness;
        }

        public void SetRainStrength(float rainStrength)
        {
            this.rainStrength = rainStrength;
        }

        public void SetGroundSmoothness(float groundSmoothness)
        {
            this.groundSmoothness = groundSmoothness;
        }
    }
}
