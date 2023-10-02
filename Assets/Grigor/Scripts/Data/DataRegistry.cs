using System;
using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Data.Credentials;
using UnityEngine;

namespace Grigor.Data
{
    [Injectable]
    public class DataRegistry : MonoBehaviour
    {
        [SerializeField] private DataStorage dataStorage;

        public List<CharacterData> CharacterData => dataStorage.CharacterData;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public CredentialWallet GetCriminalCredentials()
        {
            foreach (CharacterData characterData in CharacterData)
            {
                if (characterData.CharacterType == CharacterType.Criminal)
                {
                    return characterData.CredentialWallet;
                }
            }

            throw Log.Exception("No criminal character with credentials exists!");
        }
    }
}
