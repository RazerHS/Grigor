﻿using CardboardCore.DI;
using CardboardCore.Utilities;
using DG.Tweening;
using Grigor.Data.Credentials;
using Grigor.Gameplay.Clues;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class MindPalaceButtonInteractable : InteractableComponent, IClueListener
    {
        [SerializeField] private Transform clueObject;
        [SerializeField] private float moveObjectByY;
        [SerializeField] private CredentialType clueCredentialType;

        [Inject] private ClueRegistry clueRegistry;

        private bool clueFound;

        protected override void OnInitialized()
        {
            if (clueObject == null)
            {
                throw Log.Exception($"Clue object is not set in interactable {name}!");
            }

            RegisterClueListener();
        }

        protected override void OnInteractEffect()
        {
            if (clueFound)
            {
                clueObject.DOLocalMoveY(moveObjectByY, 1f).SetEase(Ease.Linear);
            }

            EndInteract();
        }

        public void OnClueFound(CredentialType credentialType)
        {
            if (credentialType != clueCredentialType)
            {
                return;
            }

            clueFound = true;

            Log.Write($"Clue object enabled for interactable <b>{name}</b> in mind palace!");
        }

        public void RegisterClueListener()
        {
            clueRegistry.RegisterListener(this);
        }
    }
}
