using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Data.Clues;
using UnityEngine;

namespace Grigor.Gameplay.Clues
{
    [Injectable]
    public class ClueRegistry : MonoBehaviour
    {
        private readonly List<ClueData> clues = new();
        private readonly List<ClueData> matchedClues = new();
        private readonly List<IClueListener> clueListeners = new();

        private void OnDisable()
        {
            for (int i = clues.Count - 1; i >= 0; i--)
            {
                UnregisterClue(clues[i]);
            }
        }

        private void OnClueFound(ClueData clue)
        {
            clue.ClueFoundEvent -= OnClueFound;

            foreach (IClueListener listener in clueListeners)
            {
                listener.OnClueFound(clue);
            }
        }

        public void RegisterClue(ClueData clue)
        {
            if (clues.Contains(clue))
            {
                return;
            }

            clue.ClueFoundEvent += OnClueFound;

            clues.Add(clue);
        }

        public void UnregisterClue(ClueData clue)
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

        public void RegisterMatchedClues(List<ClueData> matchesClues)
        {
            foreach (IClueListener listener in clueListeners)
            {
                listener.OnMatchedClues(matchesClues);
            }
        }
    }
}
