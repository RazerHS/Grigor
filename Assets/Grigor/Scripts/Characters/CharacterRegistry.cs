using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Characters.Components;
using Grigor.Characters.Components.Player;

namespace Grigor.Characters
{
    [Injectable]
    public class CharacterRegistry
    {
        private readonly Dictionary<string, Character> characters = new();

        private Player player;

        public Player Player => player;

        public void RegisterCharacter(string guid, Character character)
        {
            characters.TryAdd(guid, character);

            if (character is Player player)
            {
                this.player = player;
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
