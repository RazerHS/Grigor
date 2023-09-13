namespace CardboardCore.StateMachines.DemoScript.FooBarStates
{
    /// <summary>
    /// Demo code. Only use as example!
    /// </summary>
    public class DogState : State
    {
        protected override void OnEnter()
        {
            // Immediately move on to the next state for the sake of the demo.
            owningStateMachine.ToNextState();
        }

        protected override void OnExit()
        {
            
        }
    }
}