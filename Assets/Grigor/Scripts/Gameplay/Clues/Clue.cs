using System;
using CardboardCore.DI;
using Grigor.Data.Clues;
using UnityEngine;

namespace Grigor.Gameplay.Clues
{
    public class Clue : CardboardCoreBehaviour
    {
        [SerializeField] private ClueData clueData;

        public ClueData ClueData => clueData;

        [Inject] private ClueRegistry clueRegistry;

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
