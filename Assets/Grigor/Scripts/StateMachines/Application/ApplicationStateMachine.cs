using CardboardCore.StateMachines;
using Grigor.StateMachines.Application.States;

namespace Grigor.StateMachines.Application
{
    public class ApplicationStateMachine : StateMachine
    {
        public ApplicationStateMachine(bool enableDebugging) : base(enableDebugging)
        {
            SetInitialState<BootState>();

            AddStaticTransition<BootState, LoadUIState>();
            AddStaticTransition<LoadUIState, SetupGameState>();
            AddStaticTransition<SetupGameState, MainMenuState>();

            AddFreeFlowTransition<MainMenuState, GameplayState>();

            AddFreeFlowTransition<MainMenuState, SettingsState>();
            AddFreeFlowTransition<SettingsState, MainMenuState>();
        }
    }
}
