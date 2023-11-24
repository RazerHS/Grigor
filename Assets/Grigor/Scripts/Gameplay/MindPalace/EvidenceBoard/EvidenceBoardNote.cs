using CardboardCore.Utilities;
using Grigor.Data.Clues;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidenceBoardNote : MonoBehaviour
    {
        [SerializeField] private EvidenceBoardNoteType noteType;
        [SerializeField] private Transform pinTransform;
        [SerializeField] private Transform contentsParent;
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

            Vector3 pinLocalScale = pinTransform.localScale;

            //undoing the scaling done by the aspect ratio of the texture
            Vector3 newLocalScale = new Vector3(pinLocalScale.x / scale.x, pinLocalScale.y / scale.y, pinLocalScale.z / scale.z / aspect);

            pinTransform.localScale = newLocalScale;
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
        }
    }
}
