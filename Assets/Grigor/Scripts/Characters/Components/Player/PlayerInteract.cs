using System;
using CardboardCore.DI;
using Grigor.Gameplay.Interacting;
using Grigor.Gameplay.Interacting.Components;
using Grigor.Input;
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
        [ShowInInspector, ReadOnly, ColoredBoxGroup("Debugging")] private Interactable currentNearestParentInteractable;
        [ShowInInspector, ReadOnly, ColoredBoxGroup("Debugging")] private InteractableComponent previousInteraction;

        private bool forcingNextInteraction;

        public InteractableComponent PreviousInteraction => previousInteraction;
        public bool ForcingNextInteraction => forcingNextInteraction;

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
                if (currentNearestParentInteractable != null)
                {
                    currentNearestParentInteractable.DisableProximityEffect(Owner);
                }

                currentNearestParentInteractable = null;

                return;
            }

            if (!interactable.InteractingEnabled)
            {
                interactable.DisableProximityEffect(Owner);

                currentNearestParentInteractable = null;

                return;
            }

            if (currentNearestParentInteractable == null)
            {
                interactable.EnableProximityEffect(Owner);
            }

            if (currentNearestParentInteractable != null)
            {
                if (interactable != currentNearestParentInteractable)
                {
                    interactable.EnableProximityEffect(Owner);
                    currentNearestParentInteractable.DisableProximityEffect(Owner);
                }
            }

            currentNearestParentInteractable = interactable;

            if (!currentNearestParentInteractable.InteractablesChain.TryGetNextInChain(out InteractableComponent interactableComponent, false))
            {
                currentNearestParentInteractable.DisableProximityEffect(Owner);

                if (interactableComponent == null)
                {
                    return;
                }

                //edge-case when the interactable is still in the chain but already has been interacted with in the current chain
                if (interactableComponent.InteractionEnabled)
                {
                    return;
                }

                currentNearestParentInteractable.DisableInteractable();

                return;
            }

            TryInteractInRange(interactableComponent);
        }

        private void OnInteractInput()
        {
            if (!canInteract)
            {
                return;
            }

            if (currentNearestParentInteractable == null)
            {
                return;
            }

            if (!currentNearestParentInteractable.InteractablesChain.TryGetNextInChain(out previousInteraction))
            {
                return;
            }

            Interact();
        }

        private void Interact()
        {
            //first go into BeginInteraction state, then trigger the interaction
            InteractEvent?.Invoke();

            currentNearestParentInteractable.Interact(Owner);
        }

        private void TryInteractInRange(InteractableComponent interactableComponent)
        {
            if (!interactableComponent.InteractInRange)
            {
                return;
            }

            if (interactableComponent.CurrentlyInteracting)
            {
                return;
            }

            if (!currentNearestParentInteractable.InteractablesChain.TryGetNextInChain(out previousInteraction))
            {
                return;
            }

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

        public void ForceSetNextInteraction(InteractableComponent interactableComponent)
        {
            previousInteraction = interactableComponent;

            forcingNextInteraction = true;
        }

        public void ForceInteractWithNextInteraction()
        {
            Interact();

            forcingNextInteraction = false;
        }
    }
}
