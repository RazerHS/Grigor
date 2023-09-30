using System;
using System.Collections.Generic;
using Grigor.Characters.Components;
using Grigor.Utils;
using UnityEngine;

namespace Grigor.Data.Credentials
{
    [Serializable]
    public struct CredentialWallet
    {
        [SerializeField] private List<CredentialEntry> credentialEntries;

        [SerializeField, HideInInspector] private CredentialRegistry credentialRegistry;

        private Character owner;

        public List<CredentialEntry> CredentialEntries => credentialEntries;

        public void InitializeWalletForCharacter(Character owner)
        {
            this.owner = owner;

            credentialRegistry = null;
            credentialRegistry = Helper.LoadAsset("CredentialRegistry", credentialRegistry);

            credentialEntries = credentialRegistry.GetCredentials(owner.Data);
        }
    }
}
