using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class StickyNoteClue : EvidenceBoardClue
    {
        [SerializeField] private Transform leftPin;
        [SerializeField] private Transform rightPin;

        public Vector3 LeftPinPosition => leftPin.position;
        public Vector3 RightPinPosition => rightPin.position;
    }
}
