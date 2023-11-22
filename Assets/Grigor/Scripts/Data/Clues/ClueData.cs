using Grigor.Data.Credentials;
using Grigor.Data.Editor;
using UnityEngine;

namespace Grigor.Data.Clues
{
    public class ClueData : ScriptableObjectData
    {
        [SerializeField] private CredentialType credentialType;
        [SerializeField] private string clueText;

        public CredentialType CredentialType => credentialType;
        public string ClueText => clueText;
    }
}
