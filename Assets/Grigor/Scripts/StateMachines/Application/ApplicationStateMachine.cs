using CardboardCore.StateMachines;
using Grigor.StateMachines.Application.States;
using Grigor.UI.Screens;

namespace Grigor.StateMachines.Application
{
    public class ApplicationStateMachine : StateMachine
    {
        public ApplicationStateMachine(bool enableDebugging) : base(enableDebugging)
        {
            SetInitialState<BootState>();

            AddStaticTransition<BootState, LoadUIState>();
            AddStaticTransition<LoadUIState, MainMenuState>();

            AddFreeFlowTransition<MainMenuState, GameplayState>();
        }
    }
}
