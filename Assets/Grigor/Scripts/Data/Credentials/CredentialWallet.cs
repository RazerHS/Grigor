using System;
using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Data.Clues;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Credentials
{
    [Serializable]
    public class CredentialWallet
    {
        [SerializeField, GUIColor("#76989f")] private List<CredentialEntry> credentialEntries;

        public List<CredentialEntry> CredentialEntries => credentialEntries;

        public ClueData GetMatchingClue(CredentialType credentialType)
        {
            foreach (CredentialEntry credentialEntry in credentialEntries)
            {
                if (credentialEntry.CredentialType == credentialType)
                {
                    return credentialEntry.MatchingClue;
                }
            }

            throw Log.Exception($"No matching clue found for credential type {credentialType}!");
        }
    }
}
