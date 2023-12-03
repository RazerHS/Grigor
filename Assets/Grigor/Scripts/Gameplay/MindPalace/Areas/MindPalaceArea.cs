using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data.Credentials;
using Grigor.Gameplay.World.Components;
using UnityEngine;

namespace Grigor.Gameplay.MindPalace.Areas
{
    public class MindPalaceArea : CardboardCoreBehaviour
    {
        [SerializeField] private DoorInteractable door;
        [SerializeField] private CredentialType credentialType;

        [Inject] private MindPalaceManager mindPalaceManager;

        public CredentialType CredentialType => credentialType;

        protected override void OnInjected()
        {
            mindPalaceManager.RegisterMindPalaceArea(this);
        }

        protected override void OnReleased()
        {

        }

        public void UnlockArea()
        {
            Log.Write($"Unlocked area with the credential {credentialType.ToString()}!");

            door.UnlockDoor();
        }

        public void LockArea()
        {
            door.LockDoor();
        }
    }
}
