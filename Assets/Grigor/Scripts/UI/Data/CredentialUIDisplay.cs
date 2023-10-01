using System;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Data
{
    [Serializable]
    public class CredentialUIDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI credentialText;

        public void SetCredentialDisplay(string name, string value)
        {
            credentialText.text = $"{name}: {value}";
        }
    }
}
