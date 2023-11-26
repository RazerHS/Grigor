
using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public abstract class EvidenceNote : MonoBehaviour
    {
        public abstract void OnInitializeContents(EvidenceBoardNote evidenceBoardNote);
    }
}
