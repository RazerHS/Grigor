using CardboardCore.StateMachines;
using CardboardCore.Utilities;

namespace Grigor.StateMachines.Player.States
{
    public class EndInteractionState : State<PlayerStateMachine>
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
