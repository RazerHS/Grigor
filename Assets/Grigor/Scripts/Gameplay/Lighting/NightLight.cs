using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Rooms;
using Grigor.Gameplay.Time;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    public class NightLight : CardboardCoreBehaviour, ITimeEffect
    {
        [Inject] private TimeEffectRegistry timeEffectRegistry;

        private Light pointLight;
        private float originalIntensity;

        protected override void OnInjected()
        {
            if (!TryGetComponent(out pointLight))
            {
                throw Log.Exception("No light component found!");
            }

            originalIntensity = pointLight.intensity;

            RegisterTimeEffect();
        }

        protected override void OnReleased()
        {

        }

        public void OnChangedToDay()
        {
            pointLight.intensity = 0;
        }

        public void OnChangedToNight()
        {
            pointLight.intensity = originalIntensity;
        }

        public void RegisterTimeEffect()
        {
            timeEffectRegistry.Register(this);
        }
    }
}
