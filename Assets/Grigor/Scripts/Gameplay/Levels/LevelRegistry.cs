using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;

namespace Grigor.Gameplay.Time
{
    [Injectable]
    public class LevelRegistry
    {
        private Dictionary<LevelName, Level> rooms = new();

        public void Register(LevelName levelName, Level level)
        {
            rooms.TryAdd(levelName, level);
        }

        public void Unregister(LevelName levelName)
        {
            if ( !rooms.ContainsKey(levelName))
            {
                return;
            }

            rooms.Remove(levelName);
        }

        public Level GetLevel(LevelName levelName)
        {
            if (!rooms.ContainsKey(levelName))
            {
                throw Log.Exception($"Level {levelName} not found!");
            }

            return rooms[levelName];
        }

        public void DisableAllLevels()
        {
            foreach (Level level in rooms.Values)
            {
                level.DisableLevel();
            }
        }
    }
}
