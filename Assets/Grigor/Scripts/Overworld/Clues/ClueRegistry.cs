using System.Collections.Generic;
using CardboardCore.DI;
using UnityEngine;

namespace Grigor.Overworld.Clues
{
    [Injectable]
    public class ClueRegistry : MonoBehaviour
    {
        private readonly List<Clue> clues = new();
        private readonly List<IClueListener> clueListeners = new();

        private void OnDisable()
        {
            clues.ForEach(UnregisterClue);
        }

        private void OnClueFound(Clue clue)
        {
            clue.ClueFoundEvent -= OnClueFound;

            foreach (IClueListener listener in clueListeners)
            {
                listener.OnClueFound(clue.CredentialToFind);
            }
        }

        public void RegisterClue(Clue clue)
        {
            if (clues.Contains(clue))
            {
                return;
            }

            clue.ClueFoundEvent += OnClueFound;

            clues.Add(clue);
        }

        public void UnregisterClue(Clue clue)
        {
            if (!clues.Contains(clue))
            {
                return;
            }

            clue.ClueFoundEvent -= OnClueFound;

            clues.Remove(clue);
        }

        public void RegisterListener(IClueListener listener)
        {
            if (clueListeners.Contains(listener))
            {
                return;
            }

            clueListeners.Add(listener);
        }

        public void UnregisterListener(IClueListener listener)
        {
            if (!clueListeners.Contains(listener))
            {
                return;
            }

            clueListeners.Remove(listener);
        }
    }
}
