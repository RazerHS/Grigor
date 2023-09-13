using CardboardCore.StateMachines.DemoScript.FooBarStates;

namespace CardboardCore.StateMachines.DemoScript
{
    /// <summary>
    /// Demo code. Only use as example!
    /// Please refer to StaticFooBarStateMachine before having a look at this state machine
    /// </summary>
    public class FreeFlowStateMachine : StateMachine
    {
        // Enabled debugging, so we can read logs to see what's happening...
        public FreeFlowStateMachine() : base(true)
        {
            // Set the initial state we want this state machine to start in...
            SetInitialState<CatState>();

            // Create two FreeFlow transitions as based on "CatState's" code, it's possible to either
            // transition into "FooState" or "BarState"
            AddFreeFlowTransition<CatState, FooState>();
            AddFreeFlowTransition<CatState, BarState>();

            // Next we'll be re-using some code from "FooState" and "BarState". Apart from keeping your dependencies
            // and code in check, this is where the strength of our StateMachine kicks in.
            
            // Create a static transition from "FooState". As "FooState's" lcgic requires us to use a static transition.
            AddStaticTransition<FooState, CatState>();
            
            // The same goes for "BarState"
            AddStaticTransition<BarState, DogState>();

            // Creating a static transition from "DogState" back to "CatState". Now we're having an infinite loopable
            // StateMachine. This is optional of course. Just make sure to call "StateMachine.Stop" before trying to
            // transition into something that does not exist.
            AddStaticTransition<DogState, CatState>();
        }
    }
}