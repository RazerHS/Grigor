using CardboardCore.StateMachines;

namespace Grigor.StateMachines.Player.States
{
    public class IdleState : State<PlayerStateMachine>
    {
        protected override void OnEnter()
        {
            owningStateMachine.ToNextState();
        }

        protected override void OnExit()
        {

        }
    }
}
