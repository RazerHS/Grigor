using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Characters;
using Grigor.Overworld.Rooms;
using UnityEngine;

namespace Grigor.StateMachines.Gameplay.States
{
    public class StartLevelState : State
    {
        [Inject] private SpawnPointManager spawnPointManager;
        [Inject] private CharacterRegistry characterRegistry;

        protected override void OnEnter()
        {
            Vector3 spawnPosition = spawnPointManager.GetSpawnPoint(SpawnPointLocation.Start).SpawnPosition;

            characterRegistry.Player.Movement.MovePlayerTo(spawnPosition);

            characterRegistry.Player.Movement.EnableMovement();

            owningStateMachine.ToNextState();
        }

        protected override void OnExit()
        {

        }
    }
}
