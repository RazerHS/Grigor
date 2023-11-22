using System.Collections.Generic;
using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidenceBoardManager : MonoBehaviour
    {
        [SerializeField] private List<EvidenceBoardClue> clues;
        [SerializeField] private StickyNoteClue stickyNoteCluePrefab;
        [SerializeField] private ImageClue imageCluePrefab;

        private void AddClue(EvidenceBoardClue clue)
        {
            clues.Add(clue);
        }

        private void RemoveClue(EvidenceBoardClue clue)
        {
            clues.Remove(clue);
        }


    }
}
