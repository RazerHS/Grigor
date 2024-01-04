using Sirenix.OdinInspector;

namespace Grigor.Gameplay.Weather
{
    public struct PermanentWeatherData
    {
        [ShowInInspector] private float cloudDensity;
        [ShowInInspector] private float windSpeed;
        [ShowInInspector] private float windStrength;
        [ShowInInspector] private float fogAttenuationDistance;
        [ShowInInspector] private float puddleWindSpeed;
        [ShowInInspector] private float rainDropSpeed;
        [ShowInInspector] private float wetness;
        [ShowInInspector] private float rainStrength;
        [ShowInInspector] private float groundSmoothness;
        [ShowInInspector] private float cloudShapeFactor;
        [ShowInInspector] private float cloudErosionFactor;
        [ShowInInspector] private float cloudMicroErosionFactor;
        [ShowInInspector] private float cloudShapeScale;
        [ShowInInspector] private float cloudErosionScale;
        [ShowInInspector] private float exposure;
        [ShowInInspector] private float globalWindSpeed;
        [ShowInInspector] private float globalWindDirection;

        public float CloudDensity => cloudDensity;
        public float WindSpeed => windSpeed;
        public float WindStrength => windStrength;
        public float FogAttenuationDistance => fogAttenuationDistance;
        public float PuddleWindSpeed => puddleWindSpeed;
        public float RainDropSpeed => rainDropSpeed;
        public float Wetness => wetness;
        public float RainStrength => rainStrength;
        public float GroundSmoothness => groundSmoothness;
        public float CloudShapeFactor => cloudShapeFactor;
        public float CloudErosionFactor => cloudErosionFactor;
        public float CloudMicroErosionFactor => cloudMicroErosionFactor;
        public float CloudShapeScale => cloudShapeScale;
        public float CloudErosionScale => cloudErosionScale;
        public float Exposure => exposure;
        public float GlobalWindSpeed => globalWindSpeed;
        public float GlobalWindDirection => globalWindDirection;

        public void SetCloudDensity(float cloudDensity)
        {
            this.cloudDensity = cloudDensity;
        }

        public void SetGlobalWindSpeed(float globalWindSpeed)
        {
            this.globalWindSpeed = globalWindSpeed;
        }

        public void SetPuddleWindStrength(float windStrength)
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

        public void SetCloudShapeFactor(float cloudShapeFactor)
        {
            this.cloudShapeFactor = cloudShapeFactor;
        }

        public void SetCloudErosionFactor(float cloudErosionFactor)
        {
            this.cloudErosionFactor = cloudErosionFactor;
        }

        public void SetCloudMicroErosionFactor(float cloudMicroErosionFactor)
        {
            this.cloudMicroErosionFactor = cloudMicroErosionFactor;
        }

        public void SetCloudShapeScale(float cloudShapeScale)
        {
            this.cloudShapeScale = cloudShapeScale;
        }

        public void SetCloudErosionScale(float cloudErosionScale)
        {
            this.cloudErosionScale = cloudErosionScale;
        }

        public void SetExposure(float exposure)
        {
            this.exposure = exposure;
        }

        public void SetGlobalWindDirection(float globalWindDirection)
        {
            this.globalWindDirection = globalWindDirection;
        }
    }
}
