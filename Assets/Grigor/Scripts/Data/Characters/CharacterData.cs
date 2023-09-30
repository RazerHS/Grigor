using System;
using System.Collections;
using Grigor.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Characters
{
    [Serializable]
    public struct CharacterData
    {
        [SerializeField, ValueDropdown("ListCharacters"), HideLabel] private string characterName;

        [SerializeField, HideInInspector] private CharacterList characterList;

        public string CharacterName => characterName;

        private IEnumerable ListCharacters()
        {
            if (characterList == null)
            {
                characterList = Helper.LoadAsset("AllCharacters", characterList);
            }

            return characterList.AllCharacters;
        }
    }
}
