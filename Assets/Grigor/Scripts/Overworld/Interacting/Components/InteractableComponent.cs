using System;
using CardboardCore.DI;
using Sirenix.OdinInspector;
using UnityEngine;
using CharacterController = Grigor.Characters.Components.CharacterController;

namespace Grigor.Overworld.Interacting.Components
{
    public class InteractableComponent : MonoBehaviour
    {
        [SerializeField] protected bool interactInRange;
        [SerializeField] protected bool singleUse;

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

            // TO-DO: enable only the next interactable in the chain
            EnableInteraction();

            OnInitialized();
        }

        public void Dispose()
        {
            OnDisposed();
        }

        protected virtual void OnInitialized()
        {
            interactable.InRangeEvent += OnInRange;
            interactable.OutOfRangeEvent += OnOutOfRange;
        }

        protected virtual void OnDisposed()
        {
            interactable.InRangeEvent -= OnInRange;
            interactable.OutOfRangeEvent -= OnOutOfRange;
            interactable.InteractEvent -= OnInteract;

            Injector.Release(this);
        }

        protected void FindInteractable()
        {
            if (interactable == null)
            {
                interactable = GetComponent<Interactable>();
            }
        }

        private void OnInRange(CharacterController character)
        {
            wentInRange = true;

            InRangeEffect();
        }

        private void OnOutOfRange(CharacterController character)
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
    }
}
