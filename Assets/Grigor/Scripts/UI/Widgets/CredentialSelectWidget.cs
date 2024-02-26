using System;
using Grigor.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class CredentialSelectWidget : UIWidget
    {
        [SerializeField] private TextMeshProUGUI credentialNameText;
        [SerializeField] private TextMeshProUGUI optionOneText;
        [SerializeField] private TextMeshProUGUI optionTwoText;
        [SerializeField] private Button optionOneButton;
        [SerializeField] private Button optionTwoButton;

        public event Action<string> OptionSelectedEvent;

        protected override void OnShow()
        {
            optionOneButton.onClick.AddListener(OnOptionOneButtonClicked);
            optionTwoButton.onClick.AddListener(OnOptionTwoButtonClicked);

            Helper.EnableCursor();
        }

        protected override void OnHide()
        {
            optionOneButton.onClick.RemoveListener(OnOptionOneButtonClicked);
            optionTwoButton.onClick.RemoveListener(OnOptionTwoButtonClicked);

            Helper.DisableCursor();
        }

        private void OnOptionTwoButtonClicked()
        {
            OptionSelectedEvent?.Invoke(optionTwoText.text);
        }

        private void OnOptionOneButtonClicked()
        {
            OptionSelectedEvent?.Invoke(optionOneText.text);
        }

        public void SetCurrentCredential(string credentialName, string optionOne, string optionTwo)
        {
            credentialNameText.text = credentialName;
            optionOneText.text = optionOne;
            optionTwoText.text = optionTwo;
        }
    }
}
