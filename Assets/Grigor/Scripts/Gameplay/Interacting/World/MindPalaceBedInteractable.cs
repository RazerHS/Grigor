using CardboardCore.DI;
using Grigor.Characters;
using Grigor.Gameplay.Interacting.Components;
using Grigor.Gameplay.Rooms;
using Grigor.Gameplay.Time;

namespace Grigor.Gameplay.World.Components
{
    public class MindPalaceBedInteractable : InteractableComponent
    {
        [Inject] private RoomManager roomManager;
        [Inject] private CharacterRegistry characterRegistry;

        protected override void OnInteractEffect()
        {
            EndInteract();

            timeManager.ToggleTimeOfDay(0f, MovePlayerToRoom);
        }

        private void MovePlayerToRoom()
        {
            roomManager.MovePlayerToRoom(RoomName.Start, characterRegistry.Player.transform.position);
        }
    }
}
