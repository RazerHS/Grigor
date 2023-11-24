using Grigor.Data.Credentials;
using Grigor.Data.Editor;
using Grigor.Gameplay.MindPalace.EvidenceBoard;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Clues
{
    public class ClueData : ScriptableObjectData
    {
        [SerializeField] private CredentialType credentialType;
        [SerializeField] private string clueHeading;
        [SerializeField] private EvidenceBoardNoteType evidenceBoardNoteType;
        [SerializeField, ShowIf("evidenceBoardNoteType", EvidenceBoardNoteType.StickyNote)] private string evidenceText;
        [SerializeField, HorizontalGroup("picture", Width = 0.2f), ShowIf("evidenceBoardNoteType", EvidenceBoardNoteType.Picture), OnValueChanged("SetTextureSize"), PreviewField(125, ObjectFieldAlignment.Left), HideLabel, Title("Picture", TitleAlignment = TitleAlignments.Left, HorizontalLine = false)] private Sprite evidenceSprite;
        [SerializeField, VerticalGroup("picture/right"), ShowIf("evidenceBoardNoteType", EvidenceBoardNoteType.Picture), ReadOnly] private Vector2 textureSize;
        [SerializeField, VerticalGroup("picture/right"), ShowIf("evidenceBoardNoteType", EvidenceBoardNoteType.Picture)] private float upscaleFactor;

        public CredentialType CredentialType => credentialType;
        public string ClueHeading => clueHeading;
        public EvidenceBoardNoteType EvidenceBoardNoteType => evidenceBoardNoteType;
        public string EvidenceText => evidenceText;
        public Sprite EvidenceSprite => evidenceSprite;
        public Vector2 TextureSize => textureSize;
        public float UpscaleFactor => upscaleFactor;

        private void SetTextureSize()
        {
            textureSize.x = evidenceSprite.texture.width;
            textureSize.y = evidenceSprite.texture.height;
        }
    }
}
