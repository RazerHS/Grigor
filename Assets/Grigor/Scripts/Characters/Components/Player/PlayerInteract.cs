using System;
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
        [SerializeField, ColoredBoxGroup("References", false, 0.5f, 0.2f, 0,2f)] private Transform interactPoint;

        [Inject] private InteractablesRegistry interactablesRegistry;
        [Inject] private PlayerInput playerInput;

        [ShowInInspector, ReadOnly, ColoredBoxGroup("Debugging", true, 0.1f, 0.1f, 0.9f)] private bool canInteract;
        [ShowInInspector, ReadOnly, ColoredBoxGroup("Debugging")] private Interactable currentNearestInteractable;
        [ShowInInspector, ReadOnly, ColoredBoxGroup("Debugging")] private InteractableComponent previousInteraction;

        public InteractableComponent PreviousInteraction => previousInteraction;

        public event Action InteractEvent;

        protected override void OnInitialized()
        {
            playerInput.InteractInputStartedEvent += OnInteractInput;

            DisableInteract();
        }

        protected override void OnDisposed()
        {
            playerInput.InteractInputStartedEvent -= OnInteractInput;
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

            if (!interactable.InteractingEnabled)
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

            TryInteractInRange(currentNearestInteractable.GetPrimaryInteractable());
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

            previousInteraction = currentNearestInteractable.GetPrimaryInteractable();

            if (previousInteraction == null)
            {
                return;
            }

            Interact();
        }

        private void Interact()
        {
            //first go into BeginInteraction state, then trigger the interaction
            InteractEvent?.Invoke();

            currentNearestInteractable.Interact(Owner);
        }

        private void TryInteractInRange(InteractableComponent interactableComponent)
        {
            if (interactableComponent == null)
            {
                return;
            }

            if (!interactableComponent.InteractInRange)
            {
                return;
            }

            if (interactableComponent.CurrentlyInteracting)
            {
                return;
            }

            previousInteraction = currentNearestInteractable.GetPrimaryInteractable();

            Interact();
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
