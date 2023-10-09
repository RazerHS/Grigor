using CardboardCore.DI;
using Grigor.Characters;
using Grigor.Characters.Components;
using Grigor.Characters.Components.Player;
using UnityEngine;

namespace Grigor.Gameplay.Rooms
{
    [Injectable]
    public class MindPalaceManager : CardboardCoreBehaviour
    {
        [Inject] private RoomRegistry roomRegistry;
        [Inject] private CharacterRegistry characterRegistry;

        private Vector3 previousPositionInMindPalace;
        private Vector3 previousPositionInOverworld;

        private Player player;
        private Room mindPalaceRoom;

        protected override void OnInjected()
        {
            player = characterRegistry.Player;
            mindPalaceRoom = roomRegistry.GetRoom(RoomNames.MindPalace);
        }

        protected override void OnReleased()
        {

        }

        public void EnterMindPalace(Vector3 overworldPosition)
        {
            if (previousPositionInMindPalace == Vector3.zero)
            {
                previousPositionInMindPalace = mindPalaceRoom.SpawnPoint.SpawnPosition;
            }



            previousPositionInOverworld = overworldPosition;
        }
    }
}
