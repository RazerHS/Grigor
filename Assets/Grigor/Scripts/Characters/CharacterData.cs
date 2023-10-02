using System;
using Grigor.Data.Credentials;
using Grigor.Data.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Characters
{
    [Serializable]
    public class CharacterData : ScriptableObjectData
    {
        [SerializeField, DisableIn(PrefabKind.All)] private CharacterType characterType;
        [SerializeField] private CredentialWallet credentialWallet;

        public CharacterType CharacterType => characterType;
        public CredentialWallet CredentialWallet => credentialWallet;
    }
}
