using System;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters.Components;
using Grigor.Gameplay.Time;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class InteractableComponent : MonoBehaviour
    {
        [SerializeField] protected bool interactInRange;
        [SerializeField] protected bool singleUse;
        [SerializeField] protected bool useTime;

        [Inject] private TimeManager timeManager;

        protected Interactable interactable;
        protected bool wentInRange;
        protected bool interactedWith;
        protected bool currentlyInteracting;

        public bool InteractInRange => interactInRange;
        public bool CurrentlyInteracting => currentlyInteracting;
        public Interactable Interactable => interactable;

        public event Action BeginInteractionEvent;
        public event Action EndInteractionEvent;

        [OnInspectorInit]
        protected virtual void OnInspectorInit()
        {
            FindInteractable();
        }

        public void Initialize()
        {
            Injector.Inject(this);

            if (interactable == null)
            {
                interactable = GetComponent<Interactable>();
            }

            interactable.InRangeEvent += OnInRange;
            interactable.OutOfRangeEvent += OnOutOfRange;

            // TO-DO: enable only the next interactable in the chain
            EnableInteraction();

            if (useTime)
            {
                timeManager.ChangedToDayEvent += OnChangedToDay;
                timeManager.ChangedToNightEvent += OnChangedToNight;

                Log.Write($"Registering to time manager: {name}");
            }

            OnInitialized();
        }

        protected virtual void OnChangedToNight() { }

        protected virtual void OnChangedToDay() { }

        public void Dispose()
        {
            interactable.InRangeEvent -= OnInRange;
            interactable.OutOfRangeEvent -= OnOutOfRange;
            interactable.InteractEvent -= OnInteract;

            if (useTime)
            {
                timeManager.ChangedToDayEvent -= OnChangedToDay;
                timeManager.ChangedToNightEvent -= OnChangedToNight;
            }

            OnDisposed();

            Injector.Release(this);
        }

        protected virtual void OnInitialized() {}

        protected virtual void OnDisposed() {}

        protected void FindInteractable()
        {
            if (interactable == null)
            {
                interactable = GetComponent<Interactable>();
            }
        }

        private void OnInRange(Character character)
        {
            wentInRange = true;

            InRangeEffect();
        }

        private void OnOutOfRange(Character character)
        {
            OutOfRangeEffect();
        }

        protected virtual void InRangeEffect() { }

        protected virtual void OutOfRangeEffect() { }

        private void OnInteract()
        {
            Interact();
        }

        private void Interact()
        {
            BeginInteractionEvent?.Invoke();

            currentlyInteracting = true;
            interactedWith = true;

            OnInteractEffect();
        }

        protected virtual void OnInteractEffect() { }

        protected void EndInteract()
        {
            if (singleUse)
            {
                DisableInteraction();

                // TO-DO: remove this when adding chain
                interactable.DisableInteractable();
            }

            currentlyInteracting = false;

            EndInteractionEvent?.Invoke();

            EndInteractEffect();
        }

        protected virtual void EndInteractEffect() { }

        protected void EnableInteraction()
        {
            interactable.InteractEvent += OnInteract;
        }

        protected void DisableInteraction()
        {
            interactable.InteractEvent -= OnInteract;
        }

        protected void ResetInteraction()
        {
            EnableInteraction();
            interactable.ResetInteractable();
        }
    }
}
