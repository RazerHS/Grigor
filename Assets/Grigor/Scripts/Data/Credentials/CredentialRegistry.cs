using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Credentials
{
    [CreateAssetMenu(fileName = "CredentialRegistry", menuName = "Grigor/Credential Registry")]
    public class CredentialRegistry : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<CharacterData, CredentialWallet> credentials = new();

        public List<CredentialEntry> GetCredentials(CharacterData characterData)
        {
            if (!credentials.ContainsKey(characterData))
            {
                throw Log.Exception("This character does not have credentials!");
            }

            return credentials[characterData].CredentialEntries;
        }

        public List<CredentialEntry> GetCredentialsByType(CharacterType characterType)
        {
            foreach (KeyValuePair<CharacterData, CredentialWallet> entry in credentials)
            {
                if (entry.Key.CharacterType == characterType)
                {
                    return entry.Value.CredentialEntries;
                }
            }

            throw Log.Exception("There are no credentials for the criminal character!");
        }
    }
}
