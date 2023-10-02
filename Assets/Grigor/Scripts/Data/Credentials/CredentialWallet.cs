using System;
using System.Collections.Generic;
using Grigor.Characters.Components;
using Grigor.Utils;
using UnityEngine;

namespace Grigor.Data.Credentials
{
    [Serializable]
    public class CredentialWallet
    {
        [SerializeField] private List<CredentialEntry> credentialEntries;

        public List<CredentialEntry> CredentialEntries => credentialEntries;
    }
}
