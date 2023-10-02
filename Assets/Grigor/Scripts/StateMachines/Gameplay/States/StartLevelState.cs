using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Characters;
using Grigor.Overworld.Interacting;
using Grigor.Overworld.Rooms;
using Grigor.UI;
using UnityEngine;

namespace Grigor.StateMachines.Gameplay.States
{
    public class StartLevelState : State
    {
        [Inject] private SpawnPointManager spawnPointManager;
        [Inject] private CharacterRegistry characterRegistry;
        [Inject] private InteractablesRegistry interactablesRegistry;
        [Inject] private UIManager uiManager;

        private DataPodWidget dataPodWidget;

        protected override void OnEnter()
        {
            dataPodWidget = uiManager.ShowWidget<DataPodWidget>();

            interactablesRegistry.EnableInteractables();

            Vector3 spawnPosition = spawnPointManager.GetSpawnPoint(SpawnPointLocation.Start).SpawnPosition;

            characterRegistry.Player.Movement.MovePlayerTo(spawnPosition);

            characterRegistry.Player.StartStateMachine();

            owningStateMachine.ToNextState();
        }

        protected override void OnExit()
        {

        }
    }
}
