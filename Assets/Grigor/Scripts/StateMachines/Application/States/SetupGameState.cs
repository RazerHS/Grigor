using CardboardCore.DI;
using CardboardCore.StateMachines;
using RazerCore.Utils.Addressables;
using UnityEngine;

namespace Grigor.StateMachines.Application.States
{
    public class SetupGameState : State
    {
        [Inject] private AddressablesLoader addressablesLoader;

        private const int ObjectsToLoad = 2;
        private int objectsLoaded;

        protected override void OnEnter()
        {
            addressablesLoader.LoadAssetAsync<GameObject>("PlayerCharacter", OnPlayerCharacterLoaded);
            addressablesLoader.LoadAssetAsync<GameObject>("MainCamera", OnMainCameraLoaded);
        }

        protected override void OnExit()
        {

        }

        private void OnPlayerCharacterLoaded(GameObject gameObject)
        {
            GameObject playerCharacter = Object.Instantiate(gameObject);
            playerCharacter.name = "PlayerCharacter";

            CheckObjectsLoadedCount();
        }

        private void OnMainCameraLoaded(GameObject gameObject)
        {
            GameObject mainCamera = Object.Instantiate(gameObject);
            mainCamera.name = "MainCamera";

            CheckObjectsLoadedCount();
        }

        private void CheckObjectsLoadedCount()
        {
            objectsLoaded++;

            if (objectsLoaded < ObjectsToLoad)
            {
                return;
            }

            owningStateMachine.ToNextState();
        }
    }
}
