using CardboardCore.StateMachines;
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

            bool removedFromChain = chain.TryRemoveFromChain(currentInteractable);

            if (!removedFromChain)
            {
                bool nextInChainExists = chain.TryGetNextInChainAfter(currentInteractable, out nextInChain);

                if (currentInteractable.StopsChain)
                {
                    owningStateMachine.ToState<FreeRoamState>();

                    return;
                }

                if (!nextInChainExists)
                {
                    owningStateMachine.ToState<FreeRoamState>();

                    return;
                }

                if (nextInChain.StopChainIfLockedOnCurrentDay())
                {
                    owningStateMachine.ToState<FreeRoamState>();

                    return;
                }

                if (nextInChain.StopChainIfRequiredTaskNotStarted())
                {
                    owningStateMachine.ToState<FreeRoamState>();

                    return;
                }

                currentInteractable.DisableInteraction();

                nextInChain.EnableInteraction();

                ForceSetNextInteract(nextInChain);

                return;
            }

            currentInteractable.DisableInteraction();

            bool chainEnded = !chain.TryGetNextInChain(out nextInChain);

            if (chainEnded)
            {
                owningStateMachine.ToState<FreeRoamState>();

                return;
            }

            if (nextInChain.StopChainIfLockedOnCurrentDay())
            {
                owningStateMachine.ToState<FreeRoamState>();

                return;
            }

            if (nextInChain.StopChainIfRequiredTaskNotStarted())
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

            ForceSetNextInteract(nextInChain);
        }

        protected override void OnExit()
        {

        }

        private void ForceSetNextInteract(InteractableComponent nextInChain)
        {
            owningStateMachine.Owner.Interact.ForceSetNextInteraction(nextInChain);

            owningStateMachine.ToState<BeginInteractionState>();
        }
    }
}
