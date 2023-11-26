using CardboardCore.Utilities;
using Grigor.Data.Clues;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidenceBoardNote : MonoBehaviour
    {
        [SerializeField, ColoredBoxGroup("General References", false, true)] private Transform pinTransform;
        [SerializeField, ColoredBoxGroup("General References")] private Transform contentsParent;
        [SerializeField, ColoredBoxGroup("General References")] private Transform anchorToTopParent;
        [SerializeField, ColoredBoxGroup("General References")] private GameObject redHighlight;
        [SerializeField, ColoredBoxGroup("General References")] private TMP_Text clueHeadingText;

        [SerializeField, ColoredBoxGroup("Note Settings", false, true)] private EvidenceBoardNoteType noteType;
        [SerializeField, ColoredBoxGroup("Note Settings")] private EvidenceNote evidenceNote;

        [ColoredBoxGroup("Testing", false, true), Button(ButtonSizes.Large)] private void Highlight() => HighlightNote();
        [ColoredBoxGroup("Testing", false, true), Button(ButtonSizes.Large)] private void Unhighlight() => UnhighlightNote();

        private ClueData clueData;

        private Vector3 contentsDefaultScale;
        private Vector3 anchorToTopDefaultScale;
        private float clueHeadingDefaultFontSize;

        public EvidenceBoardNoteType NoteType => noteType;
        public Transform PinTransform => pinTransform;
        public Transform ContentsParent => contentsParent;
        public ClueData ClueData => clueData;

        public void ScaleContents(Vector3 scale, float upscaleFactor, float aspect)
        {
            contentsParent.localScale = scale;

            //undoing the scaling done by the aspect ratio of the texture
            Vector3 anchorToTopLocalScale = anchorToTopDefaultScale;
            Vector3 newLocalScale = new Vector3(anchorToTopLocalScale.x / scale.x, anchorToTopLocalScale.y / scale.y, anchorToTopLocalScale.z / scale.z);

            anchorToTopParent.localScale = newLocalScale;

            clueHeadingText.fontSize = clueHeadingDefaultFontSize * upscaleFactor;
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

            contentsDefaultScale = contentsParent.localScale;
            anchorToTopDefaultScale = anchorToTopParent.localScale;
            clueHeadingDefaultFontSize = clueHeadingText.fontSize;

            InitializeNoteContents();

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

        public void HideNote()
        {
            gameObject.SetActive(false);
        }

        public void RevealNote()
        {
            gameObject.SetActive(true);
        }

        public void RefreshContents()
        {
            InitializeNoteContents();
        }

        private void InitializeNoteContents()
        {
            SetHeadingText(clueData.ClueHeading);

            evidenceNote.OnInitializeContents(this);
        }
    }
}
