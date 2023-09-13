using UnityEngine;

namespace CardboardCore.StateMachines.DemoScript.Behaviours
{
    /// <summary>
    /// Demo code. Only use as example!
    /// </summary>
    public class FreeFlowStateMachineBehaviour : MonoBehaviour
    {
        private FreeFlowStateMachine freeFlowStateMachine;
        
        private void Awake()
        {
            freeFlowStateMachine = new FreeFlowStateMachine();
        }

        private void Start()
        {
            freeFlowStateMachine.Start();
        }

        private void OnDestroy()
        {
            freeFlowStateMachine.Stop();
        }
    }
}