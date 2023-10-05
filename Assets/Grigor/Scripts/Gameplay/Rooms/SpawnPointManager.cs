using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;

namespace Grigor.Gameplay.Rooms
{
    [Injectable]
    public class SpawnPointManager
    {
        private readonly Dictionary<SpawnPointLocation, SpawnPoint> spawnPoints = new();

        public void RegisterSpawnPoint(SpawnPointLocation spawnPointLocation, SpawnPoint spawnPoint)
        {
            spawnPoints.TryAdd(spawnPointLocation, spawnPoint);
        }

        public void UnregisterSpawnPoint(SpawnPointLocation spawnPointLocation)
        {
            if (!spawnPoints.ContainsKey(spawnPointLocation))
            {
                return;
            }

            spawnPoints.Remove(spawnPointLocation);
        }

        public SpawnPoint GetSpawnPoint(SpawnPointLocation spawnPointLocation)
        {
            SpawnPoint spawnPoint = spawnPoints[spawnPointLocation];

            if (spawnPoint == null)
            {
                throw Log.Exception($"Could not find spawn point with name {spawnPointLocation}!");
            }

            return spawnPoint;
        }
    }
}
