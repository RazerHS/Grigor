using CardboardCore.DI;
using UnityEngine;

namespace Grigor.Overworld.Rooms
{
    public class SpawnPoint : CardboardCoreBehaviour
    {
        [Inject] private SpawnPointManager spawnPointManager;

        [SerializeField] private SpawnPointLocation spawnPointLocation;

        private Transform spawnPointTransform;

        public Vector3 SpawnPosition => spawnPointTransform.position;

        protected override void OnInjected()
        {
            spawnPointTransform = transform;

            spawnPointManager.RegisterSpawnPoint(spawnPointLocation, this);
        }

        protected override void OnReleased()
        {
            spawnPointManager.UnregisterSpawnPoint(spawnPointLocation);
        }
    }
}
