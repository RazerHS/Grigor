using UnityEngine;

namespace CardboardCore.StateMachines.DemoScript.FooBarStates
{
    /// <summary>
    /// Demo code. Only use as example!
    /// </summary>
    public class CatState : State
    {
        private float randomValue;
        
        protected override void OnEnter()
        {
            randomValue = Random.Range(0f, 1f);

            if (randomValue > 0.5f)
            {
                owningStateMachine.ToState<FooState>();
            }
            else
            {
                owningStateMachine.ToState<BarState>();
            }
        }

        protected override void OnExit()
        {
            Debug.Log($"Exiting CatState... Random value was {randomValue}");
        }
    }
}