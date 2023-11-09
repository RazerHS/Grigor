using CardboardCore.StateMachines;
using CardboardCore.Utilities;
using Grigor.Gameplay.Interacting.Components;

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

            if (!currentInteractable.InteractionEnabled)
            {
                throw Log.Exception($"Tried to start interaction with disabled interactable <b>{currentInteractable.GetType().Name}</b> in <b>{currentInteractable.gameObject.name}</b>!");
            }

            Log.Write($"Started interaction with: <b>{currentInteractable.GetType().Name}</b> in <b>{currentInteractable.gameObject.name}</b>");

            currentInteractable.BeginInteractionEvent += OnBeginInteraction;
            currentInteractable.EndInteractionEvent += OnEndInteraction;

            if (owningStateMachine.Owner.Interact.ForcingNextInteraction)
            {
                owningStateMachine.Owner.Interact.ForceInteractWithNextInteraction();
            }
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
            Log.Write($"Ended interaction with: <b>{currentInteractable.GetType().Name}</b> in <b>{currentInteractable.gameObject.name}</b>");

            owningStateMachine.ToNextState();
        }
    }
}
