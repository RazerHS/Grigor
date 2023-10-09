using CardboardCore.DI;
using Grigor.Gameplay.Lighting;
using UnityEngine;

namespace Grigor.Gameplay.Rooms
{
    public class Room : CardboardCoreBehaviour
    {
        [SerializeField] private RoomName roomName;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject roomView;

        [Inject] private RoomRegistry roomRegistry;

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

        public void DisableRoom()
        {
            roomView.SetActive(false);
        }
    }
}
