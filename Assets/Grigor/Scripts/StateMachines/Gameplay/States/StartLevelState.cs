using CardboardCore.StateMachines;

namespace Grigor.StateMachines.Gameplay.States
{
    public class StartLevelState : State
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
