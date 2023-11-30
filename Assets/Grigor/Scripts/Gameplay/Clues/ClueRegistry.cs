﻿using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Data.Clues;
using UnityEngine;

namespace Grigor.Gameplay.Clues
{
    [Injectable]
    public class ClueRegistry : MonoBehaviour
    {
        private readonly List<Clue> clues = new();
        private readonly List<ClueData> matchedClues = new();
        private readonly List<IClueListener> clueListeners = new();

        private void OnDisable()
        {
            for (int i = clues.Count - 1; i >= 0; i--)
            {
                UnregisterClue(clues[i]);
            }
        }

        private void OnClueFound(Clue clue)
        {
            clue.ClueFoundEvent -= OnClueFound;

            foreach (IClueListener listener in clueListeners)
            {
                listener.OnClueFound(clue.ClueData);
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

        public void RegisterMatchedClues(List<ClueData> matchesClues)
        {
            foreach (IClueListener listener in clueListeners)
            {
                listener.OnMatchedClues(matchesClues);
            }
        }
    }
}
