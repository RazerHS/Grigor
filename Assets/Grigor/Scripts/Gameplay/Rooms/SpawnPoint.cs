using CardboardCore.DI;
using UnityEngine;

namespace Grigor.Gameplay.Rooms
{
    public class SpawnPoint : CardboardCoreBehaviour
    {
        [Inject] private SpawnPointManager spawnPointManager;

        [SerializeField] private SpawnPointLocation spawnPointLocation;
        [SerializeField] private Transform spawnPointTransform;

        public Vector3 SpawnPosition => spawnPointTransform.position;

        protected override void OnInjected()
        {
            spawnPointManager.RegisterSpawnPoint(spawnPointLocation, this);
        }

        protected override void OnReleased()
        {
            spawnPointManager.UnregisterSpawnPoint(spawnPointLocation);
        }
    }
}
