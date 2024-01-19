using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Interacting;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Gameplay.Time
{
    public class DisappearForeverOnNewDay : CardboardCoreBehaviour
    {
        [SerializeField, ColoredBoxGroup("Disapearrrrrrr", false, true)] private GameObject objectToDisappear;
        [SerializeField, ColoredBoxGroup("Disapearrrrrrr", false, true)] private Interactable interactableToPause;

        [Inject] private TimeManager timeManager;

        protected override void OnInjected()
        {
            if (objectToDisappear == null)
            {
                throw Log.Exception($"No object to disappear set in <b>{name}</b>!");
            }

            timeManager.NewDayEvent += OnNewDay;
        }

        protected override void OnReleased()
        {

        }

        private void OnNewDay()
        {
            objectToDisappear.SetActive(false);

            if (interactableToPause == null)
            {
                return;
            }

            interactableToPause.PauseInteractable();
        }
    }
}
