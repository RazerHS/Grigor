using System;
using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Data.Credentials;
using Grigor.Gameplay.EvidenceBoard;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Clues
{
    public class ClueData : ScriptableObjectData
    {
        [SerializeField, ColoredBoxGroup("Config", false, true)] private CredentialType credentialType;
        [SerializeField, ColoredBoxGroup("Config")] private bool appearsInDataPod;
        [SerializeField, ColoredBoxGroup("Config"), ShowIf(nameof(appearsInDataPod))] private bool typed;
        [SerializeField, ColoredBoxGroup("Evidence Board", false, true)] private EvidenceBoardNoteType evidenceBoardNoteType;
        [SerializeField, ColoredBoxGroup("Evidence Board")] private string clueHeading;
        [SerializeField, ColoredBoxGroup("Evidence Board"), ShowIf("evidenceBoardNoteType", EvidenceBoardNoteType.StickyNote)] private string evidenceText;
        [SerializeField, ColoredBoxGroup("Evidence Board"), OnValueChanged(nameof(CheckConnectingClues))] private List<ClueData> cluesToConnectTo;
        [SerializeField, HorizontalGroup("picture", Width = 0.2f), ShowIf("evidenceBoardNoteType", EvidenceBoardNoteType.Picture), OnValueChanged("SetTextureSize"), PreviewField(125, ObjectFieldAlignment.Left), HideLabel, Title("Picture", TitleAlignment = TitleAlignments.Left, HorizontalLine = false)] private Sprite evidenceSprite;
        [SerializeField, VerticalGroup("picture/right"), ShowIf("evidenceBoardNoteType", EvidenceBoardNoteType.Picture), ReadOnly] private Vector2 textureSize;
        [SerializeField, VerticalGroup("picture/right"), ShowIf("evidenceBoardNoteType", EvidenceBoardNoteType.Picture)] private float upscaleFactor = 1f;

        public CredentialType CredentialType => credentialType;
        public bool AppearsInDataPod => appearsInDataPod;
        public bool Typed => typed;
        public string ClueHeading => clueHeading;
        public EvidenceBoardNoteType EvidenceBoardNoteType => evidenceBoardNoteType;
        public List<ClueData> CluesToConnectTo => cluesToConnectTo;
        public string EvidenceText => evidenceText;
        public Sprite EvidenceSprite => evidenceSprite;
        public Vector2 TextureSize => textureSize;
        public float UpscaleFactor => upscaleFactor;

        public event Action<ClueData> ClueFoundEvent;

        private void SetTextureSize()
        {
            textureSize.x = evidenceSprite.texture.width;
            textureSize.y = evidenceSprite.texture.height;
        }

        private void CheckConnectingClues()
        {
            for (int i = cluesToConnectTo.Count - 1; i >= 0; i--)
            {
                if (cluesToConnectTo[i] != this)
                {
                    return;
                }

                cluesToConnectTo.RemoveAt(i);

                Log.Write($"Removed {clueHeading}'s connection to itself!");
            }
        }

        public void OnClueFound()
        {
            ClueFoundEvent?.Invoke(this);
        }
    }
}
