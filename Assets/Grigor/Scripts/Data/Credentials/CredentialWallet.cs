using System;
using System.Collections.Generic;
using Grigor.Characters.Components;
using Grigor.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Credentials
{
    [Serializable]
    public class CredentialWallet
    {
        [SerializeField, GUIColor("#76989f")] private List<CredentialEntry> credentialEntries;

        public List<CredentialEntry> CredentialEntries => credentialEntries;
    }
}
