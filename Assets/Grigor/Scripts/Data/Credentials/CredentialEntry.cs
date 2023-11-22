﻿using System;
using Grigor.Data.Clues;
using Grigor.Gameplay.Clues;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Credentials
{
    [Serializable]
    public struct CredentialEntry
    {
         [SerializeField, HorizontalGroup("credential", Width = 0.5f), HideLabel] private CredentialType credentialType;
         [SerializeField, HorizontalGroup("credential", Width = 0.5f), HideLabel] private string credentialValue;
         [SerializeField] private ClueData matchingClue;

        public CredentialType CredentialType => credentialType;
        public string CredentialValue => credentialValue;
    }
}
