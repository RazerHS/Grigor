using CardboardCore.Utilities;
using Grigor.Data.Clues;
using TMPro;
using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidenceBoardNote : MonoBehaviour
    {
        [SerializeField] private EvidenceBoardNoteType noteType;
        [SerializeField] private Transform pinTransform;
        [SerializeField] private Transform contentsParent;
        [SerializeField] private Transform anchorToTopParent;
        [SerializeField] private GameObject redHighlight;
        [SerializeField] private TMP_Text clueHeadingText;
        [SerializeField] private EvidenceNote evidenceNote;

        private ClueData clueData;

        public EvidenceBoardNoteType NoteType => noteType;
        public Transform PinTransform => pinTransform;
        public Transform ContentsParent => contentsParent;
        public ClueData ClueData => clueData;

        public void ScaleContents(Vector3 scale, float upscaleFactor, float aspect)
        {
            contentsParent.localScale = scale;

            //undoing the scaling done by the aspect ratio of the texture
            Vector3 anchorToTopLocalScale = anchorToTopParent.localScale;
            Vector3 newLocalScale = new Vector3(anchorToTopLocalScale.x / scale.x, anchorToTopLocalScale.y / scale.y, anchorToTopLocalScale.z / scale.z);

            anchorToTopParent.localScale = newLocalScale;

            clueHeadingText.fontSize *= upscaleFactor;
        }

        public void SetHeadingText(string text)
        {
            clueHeadingText.text = text;
        }

        public void Initialize(ClueData clueData)
        {
            this.clueData = clueData;

            if (evidenceNote == null)
            {
                throw Log.Exception($"No EvidenceNote component found on board note <b>{gameObject.name}</b>!");
            }

            evidenceNote.InitializeContents(this);

            UnhighlightNote();
        }

        public void HighlightNote()
        {
            redHighlight.SetActive(true);
        }

        public void UnhighlightNote()
        {
            redHighlight.SetActive(false);
        }
    }
}
