using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Gameplay.Time;
using Sirenix.OdinInspector;

namespace Grigor.Gameplay.Time
{
    [Injectable]
    public class TimeEffectRegistry : CardboardCoreBehaviour
    {
        [ShowInInspector, ReadOnly] private List<ITimeEffect> timeEffects = new();

        [Inject] private TimeManager timeManager;

        protected override void OnInjected()
        {
            timeManager.ChangedToDayEvent += OnChangedToDay;
            timeManager.ChangedToNightEvent += OnChangedToNight;
        }

        protected override void OnReleased()
        {
            timeManager.ChangedToDayEvent += OnChangedToDay;
            timeManager.ChangedToNightEvent += OnChangedToNight;
        }

        private void OnChangedToDay()
        {
            timeEffects.ForEach(timeEffect => timeEffect.OnChangedToDay());
        }

        private void OnChangedToNight()
        {
            timeEffects.ForEach(timeEffect => timeEffect.OnChangedToNight());
        }

        public void Register(ITimeEffect timeEffect)
        {
            if (timeEffects.Contains(timeEffect))
            {
                return;
            }

            timeEffects.Add(timeEffect);
        }

        public void Unregister(ITimeEffect timeEffect)
        {
            if (!timeEffects.Contains(timeEffect))
            {
                return;
            }

            timeEffects.Remove(timeEffect);
        }
    }
}
