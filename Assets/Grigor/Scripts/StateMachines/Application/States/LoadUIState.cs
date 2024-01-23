using CardboardCore.DI;
using CardboardCore.StateMachines;
using UnityEngine;
using RazerCore.Utils.Addressables;

namespace Grigor.StateMachines.Application.States
{
    public class LoadUIState : State
    {
        [Inject] private AddressablesLoader addressablesLoader;

        protected override void OnEnter()
        {
            addressablesLoader.LoadAssetAsync<GameObject>("UIManager", OnUIManagerLoaded);
        }

        protected override void OnExit()
        {

        }

        private void OnUIManagerLoaded(GameObject gameObject)
        {
            GameObject uiManager = Object.Instantiate(gameObject);
            uiManager.name = "UIManager";

            owningStateMachine.ToNextState();
        }
    }
}
