using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters.Components.Player;
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

        public void MovePlayerToRoom(RoomName roomName, Player player, Vector3 position = default)
        {
            Room room = GetRoom(roomName);

            if (currentRoom != null)
            {
                previousRoom = currentRoom;

                previousRoom.ExitRoom();
                previousRoomPosition = player.Movement.transform.position;
            }

            currentRoom = room;

            currentRoom.EnterRoom();

            Vector3 spawnPosition = position == Vector3.zero ? currentRoom.SpawnPoint.position : position;
            player.Movement.MovePlayerTo(spawnPosition);
        }

        public RoomName GetCurrentRoomName()
        {
            return currentRoom.RoomName;
        }
    }
}
