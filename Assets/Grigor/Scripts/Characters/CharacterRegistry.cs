using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Characters.Components;
using Grigor.Characters.Components.Player;

namespace Grigor.Characters
{
    [Injectable]
    public class CharacterRegistry
    {
        private readonly Dictionary<string, CharacterController> characters = new();

        private PlayerController player;

        public PlayerController Player => player;

        public void RegisterCharacter(string guid, CharacterController character)
        {
            characters.TryAdd(guid, character);

            if (character is PlayerController playerController)
            {
                player = playerController;
            }
        }

        public void UnregisterCharacter(string guid)
        {
            if (!characters.ContainsKey(guid))
            {
                return;
            }

            characters.Remove(guid);
        }
    }
}
