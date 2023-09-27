using CardboardCore.DI;
using CardboardCore.StateMachines;
using RazerCore.Utils.Addressables;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Grigor.StateMachines.Application.States
{
    public class SetupGameState : State
    {
        [Inject] private AddressablesLoader addressablesLoader;

        protected override void OnEnter()
        {
            addressablesLoader.LoadSceneAsync("SetupGameScene", OnSetupSceneLoaded);
        }

        protected override void OnExit()
        {

        }

        private void OnSetupSceneLoaded(SceneInstance sceneInstance)
        {
            addressablesLoader.UnloadSceneAsync("SetupGameScene", OnSetupSceneUnloaded);
        }

        private void OnSetupSceneUnloaded()
        {
            owningStateMachine.ToNextState();
        }
    }
}
