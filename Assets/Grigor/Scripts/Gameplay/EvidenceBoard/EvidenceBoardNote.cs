using System;
using CardboardCore.Utilities;
using Cinemachine;
using Grigor.Data.Clues;
using Grigor.Gameplay.Time;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Grigor.Gameplay.EvidenceBoard
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

        [SerializeField, ColoredBoxGroup("Zooming", false, true)] private CollisionData collisionData;
        [SerializeField, ColoredBoxGroup("Zooming", false, true)] private CinemachineVirtualCamera noteVirtualCamera;

        [SerializeField, ReadOnly, ColoredBoxGroup("Debug", true, true)] private bool isRevealed;

        [SerializeField, HideInInspector] private ClueData clueData;
        [SerializeField, HideInInspector] private float clueHeadingDefaultFontSize;
        [SerializeField, HideInInspector] private EvidenceBoardManager evidenceBoardManager;

        [ColoredBoxGroup("Testing", false, true), Button(ButtonSizes.Large)] private void Highlight() => HighlightNote();
        [ColoredBoxGroup("Testing", false, true), Button(ButtonSizes.Large)] private void Unhighlight() => UnhighlightNote();

        public EvidenceBoardNoteType NoteType => noteType;
        public Transform PinTransform => pinTransform;
        public Transform ContentsParent => contentsParent;
        public ClueData ClueData => clueData;
        public bool IsRevealed => isRevealed;

        public CinemachineVirtualCamera NoteVirtualCamera => noteVirtualCamera;

        public event Action<EvidenceBoardNote> NoteSelectedEvent;

        public void Initialize(ClueData clueData, EvidenceBoardManager evidenceBoardManager)
        {
            this.clueData = clueData;
            this.evidenceBoardManager = evidenceBoardManager;

            if (evidenceNote == null)
            {
                throw Log.Exception($"No EvidenceNote component found on board note <b>{gameObject.name}</b>!");
            }

            clueHeadingDefaultFontSize = clueHeadingText.fontSize;

            InitializeNoteContents();

            UnhighlightNote();
        }

        public void Dispose()
        {
            collisionData.MouseEnterEvent -= OnNoteHover;
            collisionData.MouseExitEvent -= OnNoteUnhover;
            collisionData.MouseDownEvent -= OnNoteClick;
        }

        public void ScaleContents(Vector3 scale, float upscaleFactor, float aspect)
        {
            ResetContentScale();

            contentsParent.localScale = scale;

            //undoing the scaling done by the aspect ratio of the texture
            Vector3 anchorToTopLocalScale = anchorToTopParent.localScale;
            Vector3 newLocalScale = new Vector3(anchorToTopLocalScale.x / scale.x, anchorToTopLocalScale.y / scale.y, anchorToTopLocalScale.z / scale.z);

            anchorToTopParent.localScale = newLocalScale;

            clueHeadingText.fontSize = clueHeadingDefaultFontSize * upscaleFactor;
        }

        private void ResetContentScale()
        {
            contentsParent.localScale = Vector3.one;
            anchorToTopParent.localScale = Vector3.one;
        }

        private void SetHeadingText(string text)
        {
            clueHeadingText.text = text;
        }

        public void InitializeNoteContents()
        {
            gameObject.name = clueData.name;

            SetHeadingText(clueData.ClueHeading);

            evidenceNote.OnInitializeContents(this);

            collisionData.MouseEnterEvent += OnNoteHover;
            collisionData.MouseExitEvent += OnNoteUnhover;
            collisionData.MouseDownEvent += OnNoteClick;
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
            isRevealed = false;

            gameObject.SetActive(false);
        }

        public void RevealNote()
        {
            isRevealed = true;

            gameObject.SetActive(true);
        }

        public void RefreshContents()
        {
            InitializeNoteContents();
        }

        private void OnNoteHover()
        {
            if (!evidenceBoardManager.CanSelectNote)
            {
                return;
            }
        }

        private void OnNoteUnhover()
        {
            if (!evidenceBoardManager.CanSelectNote)
            {
                return;
            }
        }

        private void OnNoteClick()
        {
            if (!evidenceBoardManager.CanSelectNote)
            {
                return;
            }

            NoteSelectedEvent?.Invoke(this);
        }
    }
}
