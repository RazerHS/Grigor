using System;
using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Data.Clues;
using Grigor.Data.Credentials;
using Grigor.Data.Tasks;
using Grigor.Utils;
using Sirenix.Utilities;
using UnityEngine;

namespace Grigor.Data
{
    [GlobalConfig("Assets/Resources/"), CreateAssetMenu(fileName = "DataStorage", menuName = "Grigor/Data Storage")]
    public class DataStorage : GlobalConfig<DataStorage>
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
