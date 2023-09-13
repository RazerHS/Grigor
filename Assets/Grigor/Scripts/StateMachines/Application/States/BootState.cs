using CardboardCore.StateMachines;

namespace Grigor.StateMachines.Application.States
{
    public class BootState : State
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
