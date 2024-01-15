﻿using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Data.Clues;
using Grigor.Data.Credentials;
using Grigor.Data.Tasks;
using UnityEngine;

namespace Grigor.Data
{
    [Injectable]
    public class DataRegistry : MonoBehaviour
    {
        [SerializeField] private DataStorage dataStorage;

        public List<CharacterData> CharacterData => dataStorage.CharacterData;
        public List<ClueData> ClueData => dataStorage.ClueData;
        public List<TaskData> TaskData => dataStorage.TaskData;

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

        public CharacterData GetDataByCharacterType(CharacterType characterType)
        {
            CharacterData characterData = CharacterData.FirstOrDefault(characterData => characterData.CharacterType == characterType);

            if (characterData == null)
            {
                throw Log.Exception($"No character data with type {characterType} exists!");
            }

            return characterData;
        }
    }
}