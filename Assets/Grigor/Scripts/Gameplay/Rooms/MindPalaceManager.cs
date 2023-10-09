using CardboardCore.DI;
using Grigor.Characters;
using Grigor.UI;
using Grigor.UI.Widgets;
using UnityEngine;

namespace Grigor.Gameplay.Rooms
{
    [Injectable]
    public class MindPalaceManager : CardboardCoreBehaviour
    {
        [Inject] private RoomRegistry roomRegistry;
        [Inject] private CharacterRegistry characterRegistry;
        [Inject] private UIManager uiManager;

        private bool insideMindPalace;
        private TransitionScreenWidget transitionScreenWidget;
        private RoomName previousRoom;

        private Vector3 previousOverworldPosition;
        private Room mindPalaceRoom;

        public bool InsideMindPalace => insideMindPalace;

        protected override void OnInjected()
        {
            transitionScreenWidget = uiManager.GetWidget<TransitionScreenWidget>();
            mindPalaceRoom = roomRegistry.GetRoom(RoomName.MindPalace);
        }

        protected override void OnReleased()
        {

        }

        public void EnterMindPalace()
        {
            insideMindPalace = true;

            // transitionScreenWidget.Show();

            previousOverworldPosition = characterRegistry.Player.Movement.transform.position;

            previousRoom = roomRegistry.GetCurrentRoomName();

            roomRegistry.MovePlayerToRoom(RoomName.MindPalace, characterRegistry.Player);
        }

        public void ExitMindPalace()
        {
            insideMindPalace = false;

            transitionScreenWidget.Show();

            roomRegistry.MovePlayerToRoom(previousRoom, characterRegistry.Player, previousOverworldPosition);
        }
    }
}
