using System;
using System.Collections.Generic;
using CardboardCore.DI;
using Codice.Client.GameUI.Status;
using DG.Tweening;
using Grigor.Characters;
using Grigor.Data.Credentials;
using Grigor.Gameplay.Interacting.Components;
using Grigor.Utils;
using NUnit.Framework.Internal;
using TinyGiantStudio.Text;
using TinyGiantStudio.Text.Example;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class DataConsentInteractable : InteractableComponent
    {
        [SerializeField] private Modular3DText consentText;
        [SerializeField] private Modular3DText credentialsText;
        [SerializeField] private Typewriter credentialsTypewriter;
        [SerializeField] private Transform viewTransform;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        [SerializeField] private float moveDownBy = 1f;
        [SerializeField] private float moveDuration = 1f;
        [SerializeField] private float secondsUntilCredentialTyped = 1f;
        [SerializeField] private float secondsBetweenFlash = 0.5f;
        [SerializeField] private Texture deniedTexture;
        [SerializeField] private Renderer pictureRenderer;

        [Inject] private CharacterRegistry characterRegistry;

        private List<PlayerCredential> playerCredentials;
        private static readonly int Image = Shader.PropertyToID("_Image");

        protected override void OnInitialized()
        {
            yesButton.interactable = false;
            noButton.interactable = false;
        }

        protected override void OnInteractEffect()
        {
            yesButton.beingPressedEvent.AddListener(OnYesButtonPressed);
            noButton.beingPressedEvent.AddListener(OnNoButtonPressed);

            yesButton.interactable = true;
            noButton.interactable = true;

            playerCredentials = characterRegistry.Player.Data.PlayerCredentials;

            EnableCursor();
        }

        private void OnNoButtonPressed()
        {
            consentText.Text = "You need to consent to proceed!";

            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            Helper.Delay(2f, () =>
            {
                consentText.gameObject.SetActive(false);

                SetDeniedTexture();
            });

            OnFinished();
        }

        private void OnYesButtonPressed()
        {
            consentText.Text = "Verifying...";

            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            credentialsTypewriter.gameObject.SetActive(true);

            DisplayCredentials();
        }

        private void FinishedTyping()
        {
            viewTransform.DOLocalMoveY(moveDownBy, moveDuration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                viewTransform.gameObject.SetActive(false);
            });

            OnFinished();
        }

        private void DisplayCredentials()
        {
            int index = 0;

            DisplayNextCredential();

            void DisplayNextCredential()
            {
                string credentialText = $"{playerCredentials[index].CredentialType}: {playerCredentials[index].Value}";

                credentialsTypewriter.ResetCurrentLetter();

                credentialsTypewriter.text = credentialText;
                credentialsTypewriter.StartTyping();

                index++;

                if (index < playerCredentials.Count)
                {
                    Helper.Delay(secondsUntilCredentialTyped, DisplayNextCredential);
                }
                else
                {
                    Helper.Delay(secondsUntilCredentialTyped, () =>
                    {
                        consentText.Text = "Credentials verified!";

                        credentialsTypewriter.gameObject.SetActive(false);

                        Helper.Delay(secondsUntilCredentialTyped, FinishedTyping);
                    });
                }
            }
        }

        private void OnFinished()
        {
            yesButton.pressCompleteEvent.RemoveListener(OnYesButtonPressed);
            noButton.pressCompleteEvent.RemoveListener(OnNoButtonPressed);

            DisableCursor();

            EndInteract();
        }

        private void EnableCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void DisableCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void SetDeniedTexture()
        {
            pictureRenderer.material.SetTexture(Image, deniedTexture);

            Helper.Delay(secondsBetweenFlash, ClearTexture);
        }

        private void ClearTexture()
        {
            pictureRenderer.material.SetTexture(Image, null);

            Helper.Delay(secondsBetweenFlash, SetDeniedTexture);
        }
    }
}
