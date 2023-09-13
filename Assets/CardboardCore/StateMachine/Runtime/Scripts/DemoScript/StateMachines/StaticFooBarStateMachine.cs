using CardboardCore.StateMachines.DemoScript.FooBarStates;

namespace CardboardCore.StateMachines.DemoScript
{
    /// <summary>
    /// Demo code. Only use as example!
    /// </summary>
    public class StaticFooBarStateMachine : StateMachine
    {
        // Enabled debugging, so we can read logs to see what's happening...
        public StaticFooBarStateMachine() : base(true)
        {
            // Set the initial state we want this state machine to start in...
            SetInitialState<FooState>();

            // Create a transition from FooState to BarState, when calling "StateMachine.ToNextState"
            // while the "StateMachine.CurrentState" is "FooState", the transition from "FooState" to "BarState" will
            // be looked up for and handled.
            AddStaticTransition<FooState, BarState>();
            
            // Create a transition from "BarState" back to "FooState". Now we can loop this state machine infinitely!
            // This is entirely optional, the StateMachine is allowed to be finite. Just make sure to call
            // "StateMachine.Stop" before trying to transition into something that does not exist.
            AddStaticTransition<BarState, FooState>();
        }
    }
}