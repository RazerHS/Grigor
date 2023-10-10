using CardboardCore.DI;
using Grigor.Characters;
using Grigor.UI;
using Grigor.UI.Widgets;
using Grigor.Utils;
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
        private TransitionWidget transitionWidget;
        private RoomName previousRoom;

        private Vector3 previousOverworldPosition;
        private Room mindPalaceRoom;

        public bool InsideMindPalace => insideMindPalace;

        protected override void OnInjected()
        {
            transitionWidget = uiManager.GetWidget<TransitionWidget>();
            mindPalaceRoom = roomRegistry.GetRoom(RoomName.MindPalace);
        }

        protected override void OnReleased()
        {

        }

        public void EnterMindPalace()
        {
            insideMindPalace = true;

            transitionWidget.Show();

            characterRegistry.Player.Movement.DisableMovement();
            characterRegistry.Player.Interact.DisableInteract();

            previousOverworldPosition = characterRegistry.Player.Movement.transform.position;

            previousRoom = roomRegistry.GetCurrentRoomName();

            Helper.Delay(transitionWidget.TransitionDuration, OnTransitionFinished);
        }

        private void OnTransitionFinished()
        {
            roomRegistry.MovePlayerToRoom(RoomName.MindPalace, characterRegistry.Player);

            characterRegistry.Player.Movement.EnableMovement();
            characterRegistry.Player.Interact.EnableInteract();
        }

        public void ExitMindPalace()
        {
            insideMindPalace = false;

            transitionWidget.Show();

            roomRegistry.MovePlayerToRoom(previousRoom, characterRegistry.Player, previousOverworldPosition);
        }
    }
}
