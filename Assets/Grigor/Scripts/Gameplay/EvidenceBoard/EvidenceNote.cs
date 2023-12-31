
using UnityEngine;

namespace Grigor.Gameplay.EvidenceBoard
{
    public abstract class EvidenceNote : MonoBehaviour
    {
        public abstract void OnInitializeContents(EvidenceBoardNote evidenceBoardNote);
    }
}
