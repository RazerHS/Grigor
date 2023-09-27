using CardboardCore.StateMachines;

namespace Grigor.StateMachines.Player.States
{
    public class FreeRoamState : State<PlayerStateMachine>
    {
        protected override void OnEnter()
        {
            owningStateMachine.Owner.Movement.EnableMovement();
            owningStateMachine.Owner.Interact.EnableInteract();

            owningStateMachine.Owner.Interact.InteractEvent += OnInteract;
        }

        protected override void OnExit()
        {
            owningStateMachine.Owner.Movement.DisableMovement();
            owningStateMachine.Owner.Interact.DisableInteract();

            owningStateMachine.Owner.Interact.InteractEvent -= OnInteract;
        }

        private void OnInteract()
        {
            owningStateMachine.ToNextState();
        }
    }
}
