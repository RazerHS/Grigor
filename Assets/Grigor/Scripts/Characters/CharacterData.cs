using System;
using Grigor.Data.Credentials;
using Grigor.Data.Editor;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Characters
{
    [Serializable]
    public class CharacterData : ScriptableObjectData
    {
        [SerializeField, HorizontalGroup("info", PaddingLeft = 3), ColoredBoxGroup("info/Info", false, 0.5f, 0.7f, 0.1f), EnumPaging, DisableIn(PrefabKind.All)] private CharacterType characterType;
        [SerializeField, HorizontalGroup("credentials", PaddingLeft = 3), ColoredBoxGroup("credentials/Credential Wallet", false, 0.2f, 0.3f, 0.9f), HideLabel] private CredentialWallet credentialWallet;

        public CharacterType CharacterType => characterType;
        public CredentialWallet CredentialWallet => credentialWallet;
    }
}
