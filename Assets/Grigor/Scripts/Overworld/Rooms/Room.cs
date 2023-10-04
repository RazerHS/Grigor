using CardboardCore.DI;
using Grigor.Overworld.Lighting;
using UnityEngine;

namespace Grigor.Overworld.Rooms
{
    public class Room : CardboardCoreBehaviour
    {
        [SerializeField] private RoomNames roomName;

        [Inject] private RoomRegistry roomRegistry;

        public LightingController Lighting { get; private set; }
        public SpawnPoint SpawnPoint { get; private set; }

        protected override void OnInjected()
        {
            Lighting = GetComponent<LightingController>();
            SpawnPoint = GetComponent<SpawnPoint>();

            roomRegistry.Register(roomName, this);
        }

        protected override void OnReleased()
        {
            roomRegistry.Unregister(roomName);
        }
    }
}
