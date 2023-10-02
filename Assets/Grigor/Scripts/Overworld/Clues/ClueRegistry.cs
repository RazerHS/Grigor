using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Data.Credentials;
using UnityEngine;

namespace Grigor.Overworld.Clues
{
    [Injectable]
    public class ClueRegistry : MonoBehaviour
    {
        private readonly List<Clue> clues = new();
        private Dictionary<IClueListener, CredentialType> clueListeners = new();

        private void OnDisable()
        {
            clues.ForEach(UnregisterClue);
        }

        private void OnClueFound(Clue clue)
        {
            clue.ClueFoundEvent -= OnClueFound;

            foreach (KeyValuePair<IClueListener, CredentialType> listener in clueListeners)
            {
                if (listener.Value != clue.CredentialToFind)
                {
                    return;
                }

                listener.Key.OnClueFound(clue.CredentialToFind);
            }
        }

        public void RegisterClue(Clue clue)
        {
            if (!clues.Contains(clue))
            {
                return;
            }

            clue.ClueFoundEvent += OnClueFound;

            clues.Add(clue);
        }

        public void UnregisterClue(Clue clue)
        {
            if (clues.Contains(clue))
            {
                return;
            }

            clue.ClueFoundEvent -= OnClueFound;

            clues.Remove(clue);
        }

        public void RegisterListener(IClueListener listener, CredentialType credentialType)
        {
            clueListeners.TryAdd(listener, credentialType);
        }

        public void UnregisterListener(IClueListener listener)
        {
            if (!clueListeners.ContainsKey(listener))
            {
                return;
            }

            clueListeners.Remove(listener);
        }
    }
}
