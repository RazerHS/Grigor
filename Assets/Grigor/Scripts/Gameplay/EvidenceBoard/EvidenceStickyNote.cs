using TMPro;
using UnityEngine;

namespace Grigor.Gameplay.EvidenceBoard
{
    public class EvidenceStickyNote : EvidenceNote
    {
        [SerializeField] private TMP_Text evidenceText;

        public override void OnInitializeContents(EvidenceBoardNote evidenceBoardNote)
        {
            evidenceText.text = evidenceBoardNote.ClueData.EvidenceText;
        }
    }
}
