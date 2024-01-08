using System;
using System.Collections.Generic;
using Grigor.Characters;
using Grigor.Data.Clues;
using Grigor.Data.Tasks;
using Grigor.Utils;
using UnityEngine;

namespace Grigor.Data
{
    [CreateAssetMenu(fileName = "DataStorage", menuName = "Grigor/Data Storage")]
    public class DataStorage : ScriptableObject
    {
        [SerializeField] private List<CharacterData> characterData;
        [SerializeField] private List<ClueData> clueData;
        [SerializeField] private List<TaskData> taskData;

        private const string characterDataPath = "Assets/Grigor/Data/Characters";
        private const string clueDataPath = "Assets/Grigor/Data/Clues";
        private const string taskDataPath = "Assets/Grigor/Data/Tasks";

        public List<CharacterData> CharacterData => characterData;
        public List<ClueData> ClueData => clueData;
        public List<TaskData> TaskData => taskData;

        public event Action OnDataRefreshed;

#if UNITY_EDITOR
        public void UpdateData()
        {
            Helper.UpdateData(characterData, characterDataPath);
            Helper.UpdateData(clueData, clueDataPath);
            Helper.UpdateData(taskData, taskDataPath);

            OnDataRefreshed?.Invoke();
        }
#endif
    }
}
