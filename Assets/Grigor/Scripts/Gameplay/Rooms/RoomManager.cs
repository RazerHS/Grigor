using System;
using CardboardCore.DI;
using UnityEngine;

namespace Grigor.Gameplay.Rooms
{
    [Injectable]
    public class RoomManager
    {
        private Vector3 previousStartRoomPlayerPosition;
        private RoomName currentRoomName;
        private RoomName previousRoomName;
        private bool playerInMindPalace;

        public Vector3 PreviousStartRoomPlayerPosition => previousStartRoomPlayerPosition;
        public RoomName CurrentRoomName => currentRoomName;
        public RoomName PreviousRoomName => previousRoomName;
        public bool PlayerInMindPalace => playerInMindPalace;

        public event Action<RoomName, RoomName> MovePlayerToRoomEvent;

        public void MovePlayerToRoom(RoomName roomName, Vector3 previousPosition)
        {
            if (currentRoomName == RoomName.Start)
            {
                previousStartRoomPlayerPosition = previousPosition;
            }

            previousRoomName = currentRoomName;
            currentRoomName = roomName;

            playerInMindPalace = currentRoomName == RoomName.MindPalace;

            MovePlayerToRoomEvent?.Invoke(previousRoomName, currentRoomName);
        }
    }
}
