using System;
using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Input;
using Grigor.UI;
using Grigor.UI.Widgets;
using UnityEngine;

namespace Grigor.Gameplay.Dialogue
{
    public class DialogueController : CardboardCoreBehaviour
    {
        [Inject] private UIManager uiManager;
        [Inject] private PlayerInput playerInput;

        private DialogueWidget dialogueWidget;

        private bool inDialogue;

        public event Action DialogueStartedEvent;
        public event Action DialogueEndedEvent;

        protected override void OnInjected()
        {
            dialogueWidget = uiManager.GetWidget<DialogueWidget>();

            playerInput.SkipInputStartedEvent += OnSkipInput;
        }

        protected override void OnReleased()
        {

        }

        private void OnSkipInput()
        {
            if (!inDialogue)
            {
                return;
            }
        }

        public void StartDialogue()
        {
            inDialogue = true;

            dialogueWidget.Show();

            DialogueStartedEvent?.Invoke();
        }

        private void EndDialogue()
        {
            dialogueWidget.Hide();

            inDialogue = false;

            DialogueEndedEvent?.Invoke();
        }
    }
}
