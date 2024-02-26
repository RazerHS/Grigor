using System;
using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Data;
using Grigor.Data.Credentials;
using Grigor.Utils.StoryGraph.Runtime;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Characters
{
    [Serializable]
    public class CharacterData : ScriptableObjectData
    {
        [SerializeField, HorizontalGroup("info", PaddingLeft = 3), ColoredBoxGroup("info/Info", false, 0.5f, 0.7f, 0.1f), EnumPaging, DisableIn(PrefabKind.All)] private CharacterType characterType;
        [SerializeField, HorizontalGroup("credentials", PaddingLeft = 3), ColoredBoxGroup("credentials/Credential Wallet", false, 0.2f, 0.3f, 0.9f), HideLabel, ShowIf("characterType", CharacterType.Criminal)] private CredentialWallet credentialWallet;
        [SerializeField, HorizontalGroup("credentials", PaddingLeft = 3), ColoredBoxGroup("credentials/Credential Wallet", false, 0.2f, 0.3f, 0.9f), ShowIf("characterType", CharacterType.Player)] private List<PlayerCredential> playerCredentialValues = new();
        [SerializeField] private DialogueGraphData characterDialogue;
        [SerializeField] private Color speakerColor;

        private List<PlayerCredential> playerCredentials;

        public CharacterType CharacterType => characterType;
        public CredentialWallet CredentialWallet => credentialWallet;
        public DialogueGraphData CharacterDialogue => characterDialogue;
        public Color SpeakerColor => speakerColor;
        public List<PlayerCredential> PlayerCredentialValues => playerCredentialValues;

        public void SetPlayerCredentials(List<PlayerCredential> playerCredentials)
        {
            if (characterType != CharacterType.Player)
            {
                Log.Error($"Trying to set player credentials on {name}, a non-player character!");

                return;
            }

            this.playerCredentials = playerCredentials;
        }
    }
}
