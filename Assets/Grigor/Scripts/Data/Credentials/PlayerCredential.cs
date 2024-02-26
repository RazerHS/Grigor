using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Credentials
{
    [Serializable]
    public class PlayerCredential
    {
        [SerializeField, HorizontalGroup("credential", Width = 0.5f), HideLabel] private CredentialType credentialType;
        [SerializeField, HorizontalGroup("credential", Width = 0.5f), HideLabel] private string value;

        public CredentialType CredentialType => credentialType;
        public string Value => value;

        public PlayerCredential(CredentialType credentialType, string value)
        {
            this.credentialType = credentialType;
            this.value = value;
        }

        public void SetValue(string value)
        {
            this.value = value;
        }
    }
}
