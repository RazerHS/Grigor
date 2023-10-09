using System.Collections.Generic;
using Grigor.Characters;
using Grigor.Utils;
using UnityEngine;

namespace Grigor.Data
{
    [CreateAssetMenu(fileName = "DataStorage", menuName = "Grigor/Data Storage")]
    public class DataStorage : ScriptableObject
    {
        [SerializeField] private List<CharacterData> characterData;

        private const string characterDataPath = "Assets/Grigor/Data/Characters";

        public List<CharacterData> CharacterData => characterData;

        public void UpdateData()
        {
            Helper.UpdateData(characterData, characterDataPath);
        }
    }
}
