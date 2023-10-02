using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Characters;
using UnityEngine;

namespace Grigor.Data
{
    [Injectable]
    public class DataRegistry : MonoBehaviour
    {
        [SerializeField] private DataStorage dataStorage;

        public List<CharacterData> CharacterData => dataStorage.CharacterData;
    }
}
