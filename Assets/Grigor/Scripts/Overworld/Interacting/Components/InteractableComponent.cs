using System;
using System.ComponentModel;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Characters.Components;
using Grigor.Characters.Components.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using CharacterController = Grigor.Characters.Components.CharacterController;

namespace Grigor.Overworld.Interacting.Components
{
    public class InteractableComponent : MonoBehaviour
    {
        [SerializeField] protected bool interactInRange;
        [SerializeField, Sirenix.OdinInspector.ReadOnly] private bool interactingEnabled;

        protected Interactable interactable;
        protected bool wentInRange;
        protected bool interactedWith;

        public bool InteractingEnabled => interactingEnabled;
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

            if (!interactingEnabled)
            {
                return;
            }

            if (interactInRange)
            {
                Interact();
            }
            else
            {
                InRangeEffect();
            }
        }

        private void OnOutOfRange(CharacterController character)
        {
            if (!interactingEnabled)
            {
                return;
            }

            OutOfRangeEffect();
        }

        protected virtual void InRangeEffect()
        {

        }

        protected virtual void OutOfRangeEffect()
        {

        }

        private void OnInteract()
        {
            Interact();
        }

        private void Interact()
        {
            BeginInteractionEvent?.Invoke();

            interactedWith = true;

            Log.Write($"Interacted with: {name} in interactable {interactable.name}!");

            OnInteractEffect();
        }

        protected virtual void OnInteractEffect()
        {

        }

        protected virtual void EndInteract()
        {
            EndInteractionEvent?.Invoke();
        }

        protected void EnableInteraction()
        {
            interactable.InteractEvent += OnInteract;
            interactingEnabled = true;
        }

        protected void DisableInteraction()
        {
            interactingEnabled = false;
            interactable.InteractEvent -= OnInteract;
        }
    }
}
