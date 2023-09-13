using CardboardCore.StateMachines;
using Grigor.StateMachines.Gameplay.States;

namespace Grigor.StateMachines.Gameplay
{
    public class GameplayStateMachine : StateMachine
    {
        public GameplayStateMachine(bool enableDebugging) : base(enableDebugging)
        {
            SetInitialState<SetupLevelState>();

            AddStaticTransition<SetupLevelState, StartLevelState>();
            AddStaticTransition<StartLevelState, LevelGameplayState>();
            AddStaticTransition<LevelGameplayState, EndLevelState>();
        }
    }
}
