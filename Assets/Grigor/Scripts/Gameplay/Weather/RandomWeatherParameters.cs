using System;
using UnityEngine;

namespace Grigor.Gameplay.Weather
{
    [Serializable]
    public class RandomWeatherParameters
    {
        [SerializeField] private float cloudShapeScale;
        [SerializeField] private float cloudErosionScale;
        [SerializeField] private float windDirection;

        public float CloudShapeScale => cloudShapeScale;
        public float CloudErosionScale => cloudErosionScale;
        public float WindDirection => windDirection;

        public void SetCloudShapeScale(float cloudShapeScale)
        {
            this.cloudShapeScale = cloudShapeScale;
        }

        public void SetCloudErosionScale(float cloudErosionScale)
        {
            this.cloudErosionScale = cloudErosionScale;
        }

        public void SetWindDirection(float windDirection)
        {
            this.windDirection = windDirection;
        }

        public void SetAllFromPreviousValues(RandomWeatherParameters previousValues)
        {
            cloudShapeScale = previousValues.cloudShapeScale;
            cloudErosionScale = previousValues.cloudErosionScale;
            windDirection = previousValues.windDirection;
        }
    }
}
