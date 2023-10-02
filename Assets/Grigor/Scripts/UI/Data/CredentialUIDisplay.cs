using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Data
{
    [Serializable]
    public class CredentialUIDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI credentialText;

        [ShowInInspector, ReadOnly] private string storedValue;

        public string StoredValue => storedValue;

        public void StoreValue(string value)
        {
            storedValue = value;
        }

        public void SetCredentialDisplay(string name, bool revealValue)
        {
            string valueToDisplay = revealValue ? storedValue : "???";

            credentialText.text = $"{name}: {valueToDisplay}";
        }
    }
}
