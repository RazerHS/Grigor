using Grigor.Data.Credentials;
using Grigor.Data.Editor;
using Grigor.Gameplay.MindPalace.EvidenceBoard;
using UnityEngine;

namespace Grigor.Data.Clues
{
    public class ClueData : ScriptableObjectData
    {
        [SerializeField] private CredentialType credentialType;
        [SerializeField] private string clueText;
        [SerializeField] private EvidenceBoardClueType evidenceBoardClueType;

        public CredentialType CredentialType => credentialType;
        public string ClueText => clueText;
        public EvidenceBoardClueType EvidenceBoardClueType => evidenceBoardClueType;
    }
}
