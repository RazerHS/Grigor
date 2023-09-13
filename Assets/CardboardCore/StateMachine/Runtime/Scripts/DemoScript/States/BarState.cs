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
    public class BarState : State
    {
        protected override async void OnEnter()
        {
            // Call some custom code specifically used for this State
            await MyCustomCode();
            
            // This will happen if StateMachine has stopped while we were awaiting
            if (owningStateMachine == null)
            {
                return;
            }
            
            // Move on to the next state. Note that the next state must have already been defined by
            // calling "StateMachine.AddStaticTransition<BarState, AnyStateYouWant>()" from the owning state machine
            owningStateMachine.ToNextState();
            
            // IMPORTANT: Don't do anything after moving to another state!!!
        }

        protected override void OnExit()
        {
            
        }

        private async Task MyCustomCode()
        {
#if CC_DI
            Log.Write("Running some code from BarState!");
#else
            Debug.Log("Running some code from BarState!");
#endif

            await Task.Delay(3000);

            // Do anything you want here...
        }
    }
}