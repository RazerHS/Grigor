using System.Collections.Generic;
using Articy.Unity;
using Articy.Unity.Interfaces;
using CardboardCore.Utilities;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Widgets
{
    public class DialogueWidget : UIWidget
    {
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI speakerText;

        protected override void OnShow()
        {

        }

        protected override void OnHide()
        {

        }

        public void SetDialogueText(string text)
        {
            dialogueText.text = text;
        }

        public void SetSpeakerText(string text)
        {
            speakerText.text = text;
        }
    }
}
