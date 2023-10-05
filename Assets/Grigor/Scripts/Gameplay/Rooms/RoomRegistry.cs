using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;

namespace Grigor.Gameplay.Rooms
{
    [Injectable]
    public class RoomRegistry
    {
        private Dictionary<RoomNames, Room> rooms = new();

        public void Register(RoomNames roomName, Room room)
        {
            rooms.TryAdd(roomName, room);
        }

        public void Unregister(RoomNames roomName)
        {
            if ( !rooms.ContainsKey(roomName))
            {
                return;
            }

            rooms.Remove(roomName);
        }

        public Room GetRoom(RoomNames roomName)
        {
            if (!rooms.ContainsKey(roomName))
            {
                throw Log.Exception($"Room {roomName} not found!");
            }

            return rooms[roomName];
        }
    }
}
