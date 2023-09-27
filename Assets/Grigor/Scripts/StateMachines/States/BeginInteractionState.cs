using CardboardCore.StateMachines;
using CardboardCore.Utilities;
using Grigor.Overworld.Interacting.Components;

namespace Grigor.StateMachines.Player.States
{
    public class BeginInteractionState : State<PlayerStateMachine>
    {
        private InteractableComponent currentInteractable;

        protected override void OnEnter()
        {
            currentInteractable = owningStateMachine.Owner.Interact.PreviousInteraction;

            if (currentInteractable == null)
            {
                throw Log.Exception("Entered interact state before interactable loaded!");
            }

            Log.Write($"Interacted with: <b>{currentInteractable.GetType().Name} in {currentInteractable.gameObject.name}</b>");

            currentInteractable.BeginInteractionEvent += OnBeginInteraction;
            currentInteractable.EndInteractionEvent += OnEndInteraction;
        }

        protected override void OnExit()
        {
            currentInteractable.BeginInteractionEvent -= OnBeginInteraction;
            currentInteractable.EndInteractionEvent -= OnEndInteraction;
        }

        private void OnBeginInteraction()
        {
        }

        private void OnEndInteraction()
        {
            Log.Write($"Ended interaction with: <b>{currentInteractable.GetType().Name} in {currentInteractable.gameObject.name}</b>");

            owningStateMachine.ToState<EndInteractionState>();
        }

    }
}
