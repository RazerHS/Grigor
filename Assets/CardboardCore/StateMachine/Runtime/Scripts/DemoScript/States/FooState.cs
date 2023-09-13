using System.Threading.Tasks;
using UnityEngine;

#if CC_DI
using CardboardCore.Utilities;
#endif

namespace CardboardCore.StateMachines.DemoScript.FooBarStates
{
    /// <summary>
    /// Demo code. Only use as example!
    /// </summary>
    public class FooState : State
    {
        protected override async void OnEnter()
        {
            // Call some custom code specifically used for this State
            await SomeRandomFooMethod();
            
            // This will happen if StateMachine has stopped while we were awaiting
            if (owningStateMachine == null)
            {
                return;
            }
            
            // Move on to the next state. Note that the next state must have already been defined by
            // calling "StateMachine.AddStaticTransition<FooState, AnyStateYouWant>()" from the owning state machine
            owningStateMachine.ToNextState();
            
            // IMPORTANT: Don't do anything after moving to another state!!!
        }

        protected override void OnExit()
        {
            
        }

        private async Task SomeRandomFooMethod()
        {
#if CC_DI
            Log.Write("Running some code from FooState!");
#else
            Debug.Log("Running some code from FooState!");
#endif
            
            await Task.Delay(3000);

            // Do anything you want here...
        }
    }
}