using System;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Data
{
    [Serializable]
    public class CredentialUIDisplay : MonoBehaviour
    {
        [SerializeField] private string credentialName;
        [SerializeField] private string credentialValue;
        [SerializeField] private TextMeshProUGUI credentialNameText;
        [SerializeField] private TextMeshProUGUI credentialValueText;

        public string CredentialName => credentialName;
        public string CredentialValue => credentialValue;

        public void DisplayCredential()
        {
            credentialNameText.text = credentialName;
            credentialValueText.text = credentialValue;
        }

        public void SetCredentialName(string name)
        {
            credentialName = name;
        }

        public void SetCredentialValue(string value)
        {
            credentialValue = value;
        }

        public void ReplaceCredential(string name, string value)
        {
            SetCredentialName(name);
            SetCredentialValue(value);
        }
    }
}
