using System;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters.Components;
using Grigor.Gameplay.Time;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grigor.Gameplay.Interacting.Components
{
    public class InteractableComponent : MonoBehaviour
    {
        [SerializeField, ColoredBoxGroup("Config", false, true)] protected bool interactInRange;
        [SerializeField, ColoredBoxGroup("Config")] protected bool removeFromChainAfterEffect;
        [SerializeField, ColoredBoxGroup("Config")] protected int indexInChain;
        [SerializeField, ColoredBoxGroup("Config")] protected bool stopsChain;

        [SerializeField, ColoredBoxGroup("Time", false, true)] protected bool hasTimeEffect;
        [SerializeField, ColoredBoxGroup("Time")] protected bool timePassesOnInteract;
        [SerializeField, ColoredBoxGroup("Time"), ShowIf(nameof(timePassesOnInteract)), Range(0, 60)] protected int minutesToPass = 1;
        [SerializeField, ColoredBoxGroup("Time"), ShowIf(nameof(timePassesOnInteract)), Range(0, 24)] protected int hoursToPass = 1;

        [SerializeField, ColoredBoxGroup("Debug", false, true), ReadOnly] private bool interactionEnabled;

        [Inject] protected TimeManager timeManager;

        protected Interactable parentInteractable;
        protected bool wentInRange;
        protected bool interactedWith;
        protected bool currentlyInteracting;

        public bool InteractInRange => interactInRange;
        public bool CurrentlyInteracting => currentlyInteracting;
        public bool StopsChain => stopsChain;
        public bool RemoveFromChainAfterEffect => removeFromChainAfterEffect;
        public int IndexInChain => indexInChain;
        public Interactable ParentInteractable => parentInteractable;

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

            if (parentInteractable == null)
            {
                parentInteractable = GetComponent<Interactable>();
            }

            parentInteractable.InRangeEvent += OnInRange;
            parentInteractable.OutOfRangeEvent += OnOutOfRange;

            if (hasTimeEffect)
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
            parentInteractable.InRangeEvent -= OnInRange;
            parentInteractable.OutOfRangeEvent -= OnOutOfRange;
            parentInteractable.InteractEvent -= OnInteract;

            if (hasTimeEffect)
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
            if (parentInteractable == null)
            {
                parentInteractable = GetComponent<Interactable>();
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
            if (timePassesOnInteract)
            {
                timeManager.PassTime(minutesToPass, hoursToPass);
            }

            currentlyInteracting = false;

            EndInteractionEvent?.Invoke();

            EndInteractEffect();
        }

        protected virtual void EndInteractEffect() { }

        public void EnableInteraction()
        {
            parentInteractable.InteractEvent += OnInteract;

            interactionEnabled = true;
        }

        public void DisableInteraction()
        {
            parentInteractable.InteractEvent -= OnInteract;

            interactionEnabled = false;
        }

        protected void ResetInteraction()
        {
            EnableInteraction();
            parentInteractable.ResetInteractable();
        }
    }
}
