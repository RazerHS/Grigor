using System;
using Grigor.Data.Credentials;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Grigor.UI.Data
{
    [Serializable]
    public class CredentialUIDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI credentialText;
        [SerializeField] private Droppable clueDroppable;
        [SerializeField] private RectTransform clueHolderTransform;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private GameObject inputPlaceholderTextObject;

        [ShowInInspector] private ClueUIDisplay attachedClueUIDisplay;

        private CredentialType credentialType;
        private bool ignoreFirstEndDrag = true;
        private Color defaultColor;
        private CredentialWallet criminalCredentialWallet;
        private bool typed;

        public CredentialType CredentialType => credentialType;
        public ClueUIDisplay AttachedClueUIDisplay => attachedClueUIDisplay;
        public RectTransform ClueHolderTransform => clueHolderTransform;
        public bool Typed => typed;
        public string ClueInput => inputField.text;

        public event Action<CredentialUIDisplay, ClueUIDisplay> CheckDroppedClueEvent;
        public event Action<CredentialUIDisplay, string> CheckInputClueStringEvent;

        public void Initialize(CredentialWallet credentialWallet)
        {
            clueDroppable.OnDropEvent += OnDrop;

            defaultColor = clueDroppable.GetComponent<Image>().color;

            DisableInputField();

            criminalCredentialWallet = credentialWallet;
        }

        public void Dispose()
        {
            clueDroppable.OnDropEvent -= OnDrop;
        }

        private void OnDrop(PointerEventData eventData)
        {
            ClueUIDisplay clueUIDisplay = eventData.pointerDrag.GetComponentInParent<ClueUIDisplay>();

            if (clueUIDisplay == null)
            {
                return;
            }

            attachedClueUIDisplay = clueUIDisplay;

            CheckDroppedClueEvent?.Invoke(this, clueUIDisplay);
        }

        public void StoreCredentialType(CredentialType credentialType)
        {
            this.credentialType = credentialType;

            typed = criminalCredentialWallet.GetMatchingClue(credentialType).Typed;

            if (!typed)
            {
                return;
            }

            EnableInputField();
            DisableDrop();
        }

        public void SetCredentialDisplay(string name, bool revealValue)
        {
            credentialText.text = $"{name}:";
        }

        public void DisableDrop()
        {
            clueDroppable.enabled = false;
        }

        public void SnapClueToHolder()
        {
            attachedClueUIDisplay.MoveParentRectTransformTo(clueHolderTransform.position);
            attachedClueUIDisplay.ResetPosition();
        }

        public void DetachClue()
        {
            attachedClueUIDisplay = null;
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

            inputField.gameObject.SetActive(false);
        }

        public void SoftDisableInputField()
        {
            inputField.interactable = false;

            inputField.onSubmit.RemoveListener(OnSubmitTypedClue);

            inputPlaceholderTextObject.SetActive(false);

            inputField.gameObject.SetActive(true);
        }

        private void OnSubmitTypedClue(string input)
        {
            CheckInputClueStringEvent?.Invoke(this, input);
        }
    }
}
