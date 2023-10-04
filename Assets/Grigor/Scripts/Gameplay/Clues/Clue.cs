using System;
using CardboardCore.DI;
using Grigor.Data.Credentials;
using UnityEngine;

namespace Grigor.Gameplay.Clues
{
    public class Clue : CardboardCoreBehaviour
    {
        [SerializeField] private CredentialType credentialToFind;

        [Inject] private ClueRegistry clueRegistry;

        public CredentialType CredentialToFind => credentialToFind;

        public event Action<Clue> ClueFoundEvent;

        protected override void OnInjected()
        {
            clueRegistry.RegisterClue(this);
        }

        protected override void OnReleased()
        {

        }

        public void FindClue()
        {
            ClueFoundEvent?.Invoke(this);

            clueRegistry.UnregisterClue(this);
        }
    }
}
