﻿using Grigor.Data.Clues;

namespace Grigor.Gameplay.Clues
{
    public interface IClueListener
    {
        public void OnClueFound(ClueData clueData);

        public void OnMatchedClue(ClueData clueMatched);

        public void RegisterClueListener();
    }
}
