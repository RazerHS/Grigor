using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Interacting;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Gameplay.Time
{
    public class DisappearDuringTimeChange : CardboardCoreBehaviour, ITimeEffect
    {
        [SerializeField, ColoredBoxGroup("Disapearrrrrrr", false, true)] private GameObject objectToDisappear;
        [SerializeField, ColoredBoxGroup("Disapearrrrrrr", false, true)] private Interactable interactableToPause;
        [SerializeField, ColoredBoxGroup("Disapearrrrrrr", false, true)] private bool disappearDuringDay;
        [SerializeField, ColoredBoxGroup("Disapearrrrrrr", false, true)] private bool disappearDuringNight;

        [Inject] private TimeEffectRegistry timeEffectRegistry;

        protected override void OnInjected()
        {
            if (objectToDisappear == null)
            {
                throw Log.Exception($"No object to disappear set in <b>{name}</b>!");
            }

            RegisterTimeEffect();
        }

        protected override void OnReleased()
        {

        }

        public void OnChangedToDay()
        {
            if (disappearDuringDay)
            {
                DisappearAndPause();

                return;
            }

            AppearAndUnpause();
        }

        public void OnChangedToNight()
        {
            if (disappearDuringNight)
            {
                DisappearAndPause();

                return;
            }

            AppearAndUnpause();
        }

        public void RegisterTimeEffect()
        {
            timeEffectRegistry.Register(this);
        }

        private void DisappearAndPause()
        {
            objectToDisappear.SetActive(false);

            if (interactableToPause == null)
            {
                return;
            }

            interactableToPause.PauseInteractable();
        }

        private void AppearAndUnpause()
        {
            objectToDisappear.SetActive(true);

            if (interactableToPause == null)
            {
                return;
            }

            interactableToPause.UnpauseInteractable();
        }
    }
}
