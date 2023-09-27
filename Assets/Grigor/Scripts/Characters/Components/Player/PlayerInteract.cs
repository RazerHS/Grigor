using System;
using System.ComponentModel;
using CardboardCore.DI;
using Grigor.Input;
using Grigor.Overworld.Interacting;
using Grigor.Overworld.Interacting.Components;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Characters.Components.Player
{
    public class PlayerInteract : CharacterComponent
    {
        [SerializeField] private Transform interactPoint;

        [Inject] private InteractablesRegistry interactablesRegistry;
        [Inject] private PlayerInput playerInput;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly, ColoredBoxGroup("Debugging", true, 0.5f, 0.5f, 0.2f)] private bool canInteract;
        [ShowInInspector, Sirenix.OdinInspector.ReadOnly, ColoredBoxGroup("Debugging")] private Interactable currentNearestInteractable;
        [ShowInInspector, Sirenix.OdinInspector.ReadOnly, ColoredBoxGroup("Debugging")] private InteractableComponent lastInteraction;

        public event Action InteractEvent;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Injector.Inject(this);

            playerInput.InteractInputStartedEvent += OnInteractInput;

            // DisableInteract();
            EnableInteract();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();

            playerInput.InteractInputStartedEvent -= OnInteractInput;

            Injector.Release(this);
        }

        private void Update()
        {
            if (!canInteract)
            {
                return;
            }

            if (!interactablesRegistry.TryGetNearestInRange(interactPoint.position, out Interactable interactable))
            {
                if (currentNearestInteractable != null)
                {
                    currentNearestInteractable.DisableProximityEffect(Owner);
                }

                currentNearestInteractable = null;

                return;
            }

            if (interactable.IsPaused)
            {
                interactable.DisableProximityEffect(Owner);

                currentNearestInteractable = null;

                return;
            }

            if (currentNearestInteractable == null)
            {
                interactable.EnableProximityEffect(Owner);
            }

            if (currentNearestInteractable != null)
            {
                if (interactable != currentNearestInteractable)
                {
                    interactable.EnableProximityEffect(Owner);
                    currentNearestInteractable.DisableProximityEffect(Owner);
                }
            }

           currentNearestInteractable = interactable;
        }

        private void OnInteractInput()
        {
            if (!canInteract)
            {
                return;
            }

            if (currentNearestInteractable == null)
            {
                return;
            }

            lastInteraction = currentNearestInteractable.GetPrimaryInteractable();

            if (lastInteraction == null)
            {
                return;
            }

            if (!lastInteraction.InteractingEnabled)
            {
                lastInteraction = null;
                return;
            }

            //first go into BeginInteraction state, then trigger the interaction
            InteractEvent?.Invoke();

            currentNearestInteractable.Interact(Owner);
        }

        public void EnableInteract()
        {
            canInteract = true;
        }

        public void DisableInteract()
        {
            canInteract = false;
        }
    }
}
