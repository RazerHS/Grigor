using System;
using Grigor.Characters;
using Grigor.Data.Clues;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Credentials
{
    [Serializable]
    public struct CredentialEntry
    {
        [SerializeField, HorizontalGroup("credential", Width = 0.5f), HideLabel] private CredentialType credentialType;
        [SerializeField, HorizontalGroup("credential", Width = 0.5f), HideLabel] private ClueData matchingClue;

        public CredentialType CredentialType => credentialType;
        public ClueData MatchingClue => matchingClue;
    }
}
