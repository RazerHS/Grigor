using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Credentials
{
    [Serializable]
    public struct CredentialEntry
    {
         [SerializeField, HorizontalGroup("credential", Width = 0.5f), HideLabel] private CredentialType credentialType;
         [SerializeField, HorizontalGroup("credential", Width = 0.5f), HideLabel] private string credentialValue;

        public CredentialType CredentialType => credentialType;
        public string CredentialValue => credentialValue;
    }
}
