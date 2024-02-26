using System;
using DG.Tweening;
using Grigor.Data.Clues;
using Grigor.Data.Credentials;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Data
{
    [Serializable]
    public class CredentialUIDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI credentialText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Image inputFieldImage;
        [SerializeField] private GameObject inputPlaceholderTextObject;
        [SerializeField] private float colorTransitionDuration = 0.5f;

        private CredentialType credentialType;
        private Color defaultColor;
        private CredentialWallet criminalCredentialWallet;
        private ClueData heldClue;

        public CredentialType CredentialType => credentialType;
        public string ClueInput => inputField.text;
        public ClueData HeldClue => heldClue;

        public event Action<CredentialUIDisplay, string> CheckInputStringEvent;

        public void Initialize(CredentialWallet credentialWallet, CredentialType credentialType, ClueData heldClue)
        {
            this.credentialType = credentialType;
            this.heldClue = heldClue;
            criminalCredentialWallet = credentialWallet;

            EnableInputField();
        }

        public void InitializePlayerCredential(CredentialType credentialType, string value)
        {
            this.credentialType = credentialType;

            credentialText.text = $"{credentialType}";
            inputField.text = value;

            MarkAsCorrect();
            DisableInputField();
        }

        public void Dispose()
        {

        }

        public void SetCredentialDisplay(string name, bool revealValue)
        {
            credentialText.text = $"{name}:";
        }

        public void EnableInputField()
        {
            inputField.interactable = true;

            inputField.onSubmit.AddListener(OnSubmitTypedClue);

            inputPlaceholderTextObject.SetActive(true);

            inputField.gameObject.SetActive(true);
        }

        public void DisableInputField()
        {
            inputField.interactable = false;

            inputField.onSubmit.RemoveListener(OnSubmitTypedClue);
        }

        private void OnSubmitTypedClue(string input)
        {
            CheckInputStringEvent?.Invoke(this, input);
        }

        public void MarkAsCorrect()
        {
            Hoverable[] inputFieldImageHoverable = GetComponentsInChildren<Hoverable>();

            foreach (Hoverable hoverable in inputFieldImageHoverable)
            {
                hoverable.Disable();
            }

            inputFieldImage.DOColor(Color.white, 0.5f).SetEase(Ease.InOutSine);
        }
    }
}
