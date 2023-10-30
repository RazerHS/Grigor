using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using UnityEngine;

namespace Grigor.Gameplay.Rooms
{
    [Injectable]
    public class RoomRegistry
    {
        private Dictionary<RoomName, Room> rooms = new();

        private Room currentRoom;
        private Room previousRoom;
        private Vector3 previousRoomPosition;

        public void Register(RoomName roomName, Room room)
        {
            rooms.TryAdd(roomName, room);
        }

        public void Unregister(RoomName roomName)
        {
            if ( !rooms.ContainsKey(roomName))
            {
                return;
            }

            rooms.Remove(roomName);
        }

        public Room GetRoom(RoomName roomName)
        {
            if (!rooms.ContainsKey(roomName))
            {
                throw Log.Exception($"Room {roomName} not found!");
            }

            return rooms[roomName];
        }

        public RoomName GetCurrentRoomName()
        {
            return currentRoom.RoomName;
        }

        public void DisableAllRooms()
        {
            foreach (Room room in rooms.Values)
            {
                room.DisableRoom(room.SpawnPoint.position, false);
            }
        }
    }
}
