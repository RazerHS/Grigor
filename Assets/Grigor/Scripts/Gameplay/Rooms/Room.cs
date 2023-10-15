using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Lighting;
using UnityEngine;

namespace Grigor.Gameplay.Rooms
{
    public class Room : CardboardCoreBehaviour
    {
        [SerializeField] private RoomName roomName;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject roomView;
        [SerializeField] private bool savePlayerPosition;

        [Inject] private RoomRegistry roomRegistry;

        private Vector3 lastPlayerPosition = Vector3.zero;
        private bool playerWasInsideRoom;

        public Transform SpawnPoint => spawnPoint;
        public RoomName RoomName => roomName;

        public LightingController Lighting { get; private set; }

        protected override void OnInjected()
        {
            Lighting = GetComponent<LightingController>();

            roomRegistry.Register(roomName, this);
        }

        protected override void OnReleased()
        {
            roomRegistry.Unregister(roomName);
        }

        public void EnableRoom()
        {
            roomView.SetActive(true);
        }

        public void DisableRoom(Vector3 playerPosition, bool savePosition)
        {
            if (savePosition)
            {
                playerWasInsideRoom = true;
                lastPlayerPosition = playerPosition;
            }

            roomView.SetActive(false);
        }

        public Vector3 GetSpawnPosition()
        {
            if (savePlayerPosition)
            {
                return playerWasInsideRoom ? lastPlayerPosition : spawnPoint.position;
            }

            return spawnPoint.position;
        }
    }
}
