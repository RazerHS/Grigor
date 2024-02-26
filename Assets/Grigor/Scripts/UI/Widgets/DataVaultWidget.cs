using System;
using System.Collections.Generic;
using Grigor.Data.Credentials;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class DataVaultWidget : UIWidget
    {
        [SerializeField] private TextMeshProUGUI credentialText;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_InputField inputField;

        private List<CredentialType> credentialList;
        private int currentCredentialIndex;

        public event Action <CredentialType, string> CredentialChangedEvent;
        public event Action BackButtonPressedEvent;

        protected override void OnShow()
        {
            leftButton.onClick.AddListener(OnLeftButtonClicked);
            rightButton.onClick.AddListener(OnRightButtonClicked);
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            inputField.onEndEdit.AddListener(OnEndEdit);
            backButton.onClick.AddListener(OnBackButtonClicked);

            EnableCursor();
        }

        private void OnBackButtonClicked()
        {
            currentCredentialIndex = 0;

            inputField.text = "";

            BackButtonPressedEvent?.Invoke();
        }

        private void OnEndEdit(string value)
        {
            confirmButton.interactable = !value.IsNullOrWhitespace();
        }

        protected override void OnHide()
        {
            leftButton.onClick.RemoveListener(OnLeftButtonClicked);
            rightButton.onClick.RemoveListener(OnRightButtonClicked);
            confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            inputField.onEndEdit.RemoveListener(OnEndEdit);
            backButton.onClick.RemoveListener(OnBackButtonClicked);

            currentCredentialIndex = 0;

            DisableCursor();
        }

        private void OnConfirmButtonClicked()
        {
            CredentialChangedEvent?.Invoke(credentialList[currentCredentialIndex], inputField.text);

            inputField.text = "";
        }

        private void OnRightButtonClicked()
        {
            currentCredentialIndex++;
            currentCredentialIndex = currentCredentialIndex >= credentialList.Count ? 0 : currentCredentialIndex;

            UpdateCredentialText();
        }

        private void OnLeftButtonClicked()
        {
            currentCredentialIndex--;
            currentCredentialIndex = currentCredentialIndex < 0 ? credentialList.Count - 1 : currentCredentialIndex;

            UpdateCredentialText();
        }

        private void UpdateCredentialText()
        {
            credentialText.text = credentialList[currentCredentialIndex].ToString();
        }

        private void EnableCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void DisableCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void InitializeCredentialList(List<CredentialType> credentialList)
        {
            this.credentialList = credentialList;

            UpdateCredentialText();
        }
    }
}
