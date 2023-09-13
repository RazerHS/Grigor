using UnityEngine;

namespace Grigor.StateMachines.Application
{
    public class ApplicationStateMachineController : MonoBehaviour
    {
        private ApplicationStateMachine applicationStateMachine;

        private void Awake()
        {
            applicationStateMachine = new ApplicationStateMachine(true);

            applicationStateMachine.Start();

            DontDestroyOnLoad(this);
        }
    }
}
