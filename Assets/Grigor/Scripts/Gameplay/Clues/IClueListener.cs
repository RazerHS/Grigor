using System.Collections.Generic;
using Grigor.Data.Clues;

namespace Grigor.Gameplay.Clues
{
    public interface IClueListener
    {
        public void OnClueFound(ClueData clueData);

        public void OnMatchedClues(List<ClueData> matchedClues);

        public void RegisterClueListener();
    }
}
