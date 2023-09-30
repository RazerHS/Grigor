using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Data.Characters;
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
    }
}
