using CardboardCore.DI;
using CardboardCore.Utilities;
using DG.Tweening;
using Grigor.Gameplay.Rooms;
using Grigor.Gameplay.Time;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    public class NightLight : CardboardCoreBehaviour, ITimeEffect
    {
        [SerializeField] private float fadeDuration = 0.5f;

        [Inject] private TimeEffectRegistry timeEffectRegistry;

        private Light pointLight;
        private float originalIntensity;

        protected override void OnInjected()
        {
            if (!TryGetComponent(out pointLight))
            {
                throw Log.Exception($"No light component found in {name}!");
            }

            originalIntensity = pointLight.intensity;

            RegisterTimeEffect();
        }

        protected override void OnReleased()
        {

        }

        public void OnChangedToDay()
        {
            pointLight.DOIntensity(0f, fadeDuration).SetEase(Ease.OutSine);
        }

        public void OnChangedToNight()
        {
            pointLight.DOIntensity(originalIntensity, fadeDuration).SetEase(Ease.OutSine);
        }

        public void RegisterTimeEffect()
        {
            timeEffectRegistry.Register(this);
        }
    }
}
