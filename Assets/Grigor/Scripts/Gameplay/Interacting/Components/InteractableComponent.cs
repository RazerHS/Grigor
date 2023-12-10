using System;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters.Components;
using Grigor.Gameplay.Time;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class InteractableComponent : MonoBehaviour
    {
        [PropertyTooltip("Should this component be triggered when the player is in range?")]
        [SerializeField, ColoredBoxGroup("Config", false, true)] protected bool interactInRange;

        [PropertyTooltip("Will this component remove itself from the chain so that it cannot be interacted with again after the first interaction?")]
        [SerializeField, ColoredBoxGroup("Chain", false, true), InfoBox("Stays in chain and also stops the chain!", InfoMessageType.Warning, nameof(CheckConfig))] protected bool removeFromChainAfterEffect;

        [PropertyTooltip("Should this interaction stop the interaction chain and let the player roam?")]
        [SerializeField, ColoredBoxGroup("Chain"), HideIf(nameof(IsOnlyInteractableInChain))] protected bool stopsChain;

        [PropertyTooltip("The element index of this component in the parent interactable chain. You can change this index by clicking the arrows to the right of each element in the chain.")]
        [SerializeField, ColoredBoxGroup("Chain"), HideIf(nameof(IsOnlyInteractableInChain)), ReadOnly] protected int indexInChain;

        [PropertyTooltip("Does this component have an effect when the time of day changes to day and night?")]
        [SerializeField, ColoredBoxGroup("Time", false, true)] protected bool hasTimeEffect;

        [PropertyTooltip("Should time pass after this interaction triggers?")]
        [SerializeField, ColoredBoxGroup("Time")] protected bool timePassesOnInteract;

        [SerializeField, ColoredBoxGroup("Time"), ShowIf(nameof(timePassesOnInteract)), Range(0, 60)] protected int minutesToPass = 1;
        [SerializeField, ColoredBoxGroup("Time"), ShowIf(nameof(timePassesOnInteract)), Range(0, 24)] protected int hoursToPass = 1;

        [SerializeField, ColoredBoxGroup("Debug", false, true), ReadOnly] private bool interactionEnabled;
        [ShowInInspector, ColoredBoxGroup("Debug", false, true), ReadOnly] private bool interactedWithInCurrentChain;

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
        public bool InteractionEnabled => interactionEnabled;
        public bool InteractedWithInCurrentChain => interactedWithInCurrentChain;

        public event Action BeginInteractionEvent;
        public event Action EndInteractionEvent;

        private bool IsOnlyInteractableInChain()
        {
            return parentInteractable.InteractablesChain.InteractablesInChainCount <= 1;
        }

        private bool CheckConfig()
        {
            return stopsChain && !removeFromChainAfterEffect;
        }

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

            interactedWithInCurrentChain = true;

            EndInteractionEvent?.Invoke();

            EndInteractEffect();
        }

        protected virtual void EndInteractEffect() { }

        public void EnableInteraction()
        {
            if (!interactionEnabled)
            {
               parentInteractable.InteractEvent += OnInteract;
            }

            interactionEnabled = true;
        }

        public void DisableInteraction()
        {
            if (interactionEnabled)
            {
                parentInteractable.InteractEvent -= OnInteract;
            }

            interactionEnabled = false;
        }

        public void OnCurrentChainEnded()
        {
            interactedWithInCurrentChain = false;

            EnableInteraction();
        }

        protected void ResetInteraction()
        {
            EnableInteraction();

            parentInteractable.ResetInteractable();
        }

        public void SetDefaultIndexInChain(int index)
        {
            indexInChain = index;
        }
    }
}
