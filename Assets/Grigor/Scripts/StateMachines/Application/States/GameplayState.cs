using CardboardCore.DI;
using CardboardCore.StateMachines;
using UnityEngine.ResourceManagement.ResourceProviders;
using Grigor.Addressables;
using Grigor.StateMachines.Gameplay;

namespace Grigor.StateMachines.Application.States
{
    public class GameplayState : State
    {
        [Inject] private AddressablesLoader addressablesLoader;

        private StateMachine gameplayStateMachine;

        protected override void OnEnter()
        {
            addressablesLoader.LoadSceneAsync("LevelScene", OnLevelSceneLoaded);
        }

        protected override void OnExit()
        {
            addressablesLoader.UnloadSceneAsync("LevelScene", OnLevelSceneUnloaded);
        }

        private void OnLevelSceneLoaded(SceneInstance sceneInstance)
        {
            gameplayStateMachine = new GameplayStateMachine(true);
            gameplayStateMachine.Start();
        }

        private void OnLevelSceneUnloaded()
        {
            gameplayStateMachine.Stop();
            gameplayStateMachine = null;
        }
    }
}
