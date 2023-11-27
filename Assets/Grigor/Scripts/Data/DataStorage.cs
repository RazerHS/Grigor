using System;
using System.Collections.Generic;
using Grigor.Characters;
using Grigor.Data.Clues;
using Grigor.Utils;
using UnityEngine;

namespace Grigor.Data
{
    [CreateAssetMenu(fileName = "DataStorage", menuName = "Grigor/Data Storage")]
    public class DataStorage : ScriptableObject
    {
        [SerializeField] private List<CharacterData> characterData;
        [SerializeField] private List<ClueData> clueData;

        private const string characterDataPath = "Assets/Grigor/Data/Characters";
        private const string clueDataPath = "Assets/Grigor/Data/Clues";

        public List<CharacterData> CharacterData => characterData;
        public List<ClueData> ClueData => clueData;

        public event Action OnDataRefreshed;

        public void UpdateData()
        {
            Helper.UpdateData(characterData, characterDataPath);
            Helper.UpdateData(clueData, clueDataPath);

            OnDataRefreshed?.Invoke();
        }
    }
}
