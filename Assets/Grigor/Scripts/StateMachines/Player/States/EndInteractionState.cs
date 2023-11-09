using CardboardCore.StateMachines;
using CardboardCore.Utilities;
using Grigor.Gameplay.Interacting;
using Grigor.Gameplay.Interacting.Components;

namespace Grigor.StateMachines.Player.States
{
    public class EndInteractionState : State<PlayerStateMachine>
    {
        private InteractableComponent currentInteractable;

        protected override void OnEnter()
        {
            InteractableComponent nextInChain = null;

            currentInteractable = owningStateMachine.Owner.Interact.PreviousInteraction;
            InteractablesChain chain = currentInteractable.ParentInteractable.InteractablesChain;

            if (!chain.TryRemoveFromChain(currentInteractable))
            {
                if (currentInteractable.StopsChain)
                {
                    throw Log.Exception($"Interactable {currentInteractable.name} stayed in chain but also stopped it!");
                }

                if (!chain.TryGetNextInChainAfter(currentInteractable, out nextInChain))
                {
                    owningStateMachine.ToState<FreeRoamState>();

                    return;
                }

                InteractWithNextInChain(nextInChain);

                return;
            }

            currentInteractable.DisableInteraction();

            bool chainEnded = !chain.TryGetNextInChain(out nextInChain);

            if (chainEnded)
            {
                owningStateMachine.ToState<FreeRoamState>();

                return;
            }

            nextInChain.EnableInteraction();

            if (currentInteractable.StopsChain)
            {
                owningStateMachine.ToState<FreeRoamState>();

                return;
            }

            InteractWithNextInChain(nextInChain);
        }

        protected override void OnExit()
        {

        }

        private void InteractWithNextInChain(InteractableComponent nextInChain)
        {
            owningStateMachine.ToState<BeginInteractionState>();

            owningStateMachine.Owner.Interact.InteractWithNextInChain(nextInChain);
        }
    }
}
