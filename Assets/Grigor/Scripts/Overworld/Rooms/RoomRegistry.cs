using System.Collections.Generic;
using CardboardCore.DI;

namespace Grigor.Overworld.Rooms
{
    [Injectable]
    public class RoomRegistry
    {
        private Dictionary<RoomNames, Room> rooms;

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
    }
}
