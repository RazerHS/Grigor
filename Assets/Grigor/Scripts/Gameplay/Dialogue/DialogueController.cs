using System;
using System.Collections.Generic;
// using Articy.GrigorArticy;
using Articy.Unity;
using Articy.Unity.Interfaces;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Input;
using Grigor.UI;
using Grigor.UI.Widgets;
using UnityEngine;

namespace Grigor.Gameplay.Dialogue
{
    [Injectable, RequireComponent(typeof(ArticyFlowPlayer))]
    public class DialogueController : CardboardCoreBehaviour, IArticyFlowPlayerCallbacks
    {
        [Inject] private UIManager uiManager;
        [Inject] private PlayerInput playerInput;

        private ArticyFlowPlayer flowPlayer;
        private DialogueWidget dialogueWidget;

        private bool inDialogue;

        public event Action DialogueStartedEvent;
        public event Action DialogueEndedEvent;

        protected override void OnInjected()
        {
            flowPlayer = GetComponent<ArticyFlowPlayer>();
            dialogueWidget = uiManager.GetWidget<DialogueWidget>();

            playerInput.SkipInputStartedEvent += OnSkipInput;
        }

        protected override void OnReleased()
        {

        }

        private string GetDialogue(IFlowObject flowObject)
        {
            if (flowObject is not IObjectWithText text)
            {
                throw Log.Exception($"Flow object {flowObject} does not have text!");
            }

            return text.Text;
        }

        // private string GetSpeaker(IFlowObject flowObject)
        // {
        //     if (flowObject is not IObjectWithSpeaker speaker)
        //     {
        //         throw Log.Exception($"Flow object {flowObject} does not have a speaker!");
        //     }
        //
        //     Entity speakerEntity = speaker.Speaker as Entity;
        //
        //     if (speakerEntity == null)
        //     {
        //         throw Log.Exception($"Speaker {speaker.Speaker} is not an entity!");
        //     }
        //
        //     return speakerEntity.DisplayName;
        // }

        private void OnSkipInput()
        {
            if (!inDialogue)
            {
                return;
            }

            flowPlayer.Play();
        }

        public void StartDialogue(ArticyObject articyObject)
        {
            inDialogue = true;

            flowPlayer.StartOn = articyObject;

            dialogueWidget.Show();

            DialogueStartedEvent?.Invoke();
        }

        private void EndDialogue()
        {
            flowPlayer.FinishCurrentPausedObject();

            dialogueWidget.Hide();

            inDialogue = false;

            DialogueEndedEvent?.Invoke();
        }

        // Note: this method gets called after every node
        public void OnFlowPlayerPaused(IFlowObject flowObject)
        {
            dialogueWidget.SetDialogueText(GetDialogue(flowObject));
            // dialogueWidget.SetSpeakerText(GetSpeaker(flowObject));
        }

        public void OnBranchesUpdated(IList<Branch> branches)
        {
            foreach (Branch branch in branches)
            {
                if (branch.Target is IDialogueFragment)
                {
                    continue;
                }

                EndDialogue();

                return;
            }
        }
    }
}
