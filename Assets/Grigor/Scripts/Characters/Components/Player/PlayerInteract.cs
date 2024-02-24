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

            if (interactable.IsPaused)
            {
                interactable.DisableProximityEffect(Owner);

                currentNearestParentInteractable = null;

                return;
            }


            //this is the main enable proximity effect so that it only happens once
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
                if (!currentNearestParentInteractable.InRange)
                {
                    currentNearestParentInteractable.DisableProximityEffect(Owner);
                }

                if (interactableComponent == null)
                {
                    currentNearestParentInteractable.DisableProximityEffect(Owner);

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

            if (interactableComponent.StopChainIfLockedOnCurrentDay())
            {
                currentNearestParentInteractable.DisableProximityEffect(Owner);

                return;
            }

            if (interactableComponent.StopChainIfRequiredTaskNotStarted())
            {
                currentNearestParentInteractable.DisableProximityEffect(Owner);

                return;
            }

            currentNearestParentInteractable.EnableProximityEffect(Owner);

            if (!TryInteractInRange(interactableComponent))
            {
                return;
            }

            //case where the interactable that triggers in range is still in range, but the interaction is happening and the visuals should disappear
            currentNearestParentInteractable.DisableProximityEffectWithoutNotify(Owner);
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

            if (previousInteraction.StopChainIfLockedOnCurrentDay())
            {
                return;
            }

            if (previousInteraction.StopChainIfRequiredTaskNotStarted())
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

        private bool TryInteractInRange(InteractableComponent interactableComponent)
        {
            if (!interactableComponent.InteractInRange)
            {
                return false;
            }

            if (interactableComponent.CurrentlyInteracting)
            {
                return false;
            }

            if (interactableComponent.StillInRangeAfterInteraction)
            {
                return false;
            }

            if (!currentNearestParentInteractable.InteractablesChain.TryGetNextInChain(out previousInteraction))
            {
                return false;
            }

            Interact();

            return true;
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
