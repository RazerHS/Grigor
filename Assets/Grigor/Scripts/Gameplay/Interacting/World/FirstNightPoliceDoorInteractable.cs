namespace Grigor.Gameplay.World.Components
{
    public class FirstNightPoliceDoorInteractable : DoorInteractable
    {
        protected override void OnChangedToDay()
        {
            LockDoor();
            CloseDoor();
        }

        protected override void OnChangedToNight()
        {
            UnlockDoor();
            OpenDoor();
        }
    }
}
