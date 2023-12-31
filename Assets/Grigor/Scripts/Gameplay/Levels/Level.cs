using CardboardCore.DI;
using Grigor.Gameplay.Time.Lighting;
using UnityEngine;

namespace Grigor.Gameplay.Time
{
    public class Level : CardboardCoreBehaviour
    {
        [SerializeField] private LevelName levelName;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject roomView;

        [Inject] private LevelRegistry levelRegistry;

        public Transform SpawnPoint => spawnPoint;
        public LevelName LevelName => levelName;

        public LevelLighting LevelLighting { get; private set; }

        protected override void OnInjected()
        {
            LevelLighting = GetComponentInChildren<LevelLighting>(true);

            levelRegistry.Register(levelName, this);
        }

        protected override void OnReleased()
        {
            levelRegistry.Unregister(levelName);
        }

        public void EnableLevel()
        {
            roomView.SetActive(true);
        }

        public void DisableLevel()
        {
            roomView.SetActive(false);
        }
    }
}
