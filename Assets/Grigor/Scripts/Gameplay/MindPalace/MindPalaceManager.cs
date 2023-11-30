using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Data.Clues;
using Grigor.Gameplay.Clues;
using Grigor.Gameplay.MindPalace.Areas;
using Sirenix.OdinInspector;

namespace Grigor.Gameplay.MindPalace
{
    [Injectable]
    public class MindPalaceManager : CardboardCoreBehaviour, IClueListener
    {
        [Inject] private ClueRegistry clueRegistry;

        [ShowInInspector] private List<MindPalaceArea> areas = new();

        protected override void OnInjected()
        {
            RegisterClueListener();
        }

        protected override void OnReleased()
        {

        }

        public void OnClueFound(ClueData clueData)
        {

        }

        public void OnMatchedClues(List<ClueData> matchedClues)
        {
            foreach (MindPalaceArea area in areas)
            {
                foreach (ClueData clueData in matchedClues)
                {
                    if (area.CredentialType != clueData.CredentialType)
                    {
                        continue;
                    }

                    area.UnlockArea();
                }
            }
        }

        public void RegisterClueListener()
        {
            clueRegistry.RegisterListener(this);
        }

        public void RegisterMindPalaceArea(MindPalaceArea area)
        {
            areas.Add(area);
        }
    }
}
