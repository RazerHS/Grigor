using System.Collections.Generic;
using UnityEngine;

namespace Grigor.Data.Characters
{
    [CreateAssetMenu(fileName = "AllCharacters", menuName = "Grigor/CharacterData List")]
    public class CharacterList : ScriptableObject
    {
        [SerializeField] private List<string> allCharacters = new();

        public List<string> AllCharacters => allCharacters;
    }
}
