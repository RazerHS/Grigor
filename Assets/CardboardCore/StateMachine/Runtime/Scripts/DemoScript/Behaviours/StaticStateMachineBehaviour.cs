using CardboardCore.StateMachines.DemoScript;
using UnityEngine;

/// <summary>
/// Demo code. Only use as example!
/// </summary>
public class StaticStateMachineBehaviour : MonoBehaviour
{
    private StaticFooBarStateMachine staticFooBarStateMachine;

    private void Awake()
    {
        staticFooBarStateMachine = new StaticFooBarStateMachine();
    }

    private void Start()
    {
        staticFooBarStateMachine.Start();
    }

    private void OnDestroy()
    {
        staticFooBarStateMachine.Stop();
    }
}
