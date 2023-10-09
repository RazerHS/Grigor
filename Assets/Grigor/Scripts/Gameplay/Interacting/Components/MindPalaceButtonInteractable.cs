using CardboardCore.DI;
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
        }

        protected override void OnInteractEffect()
        {
            if (clueFound)
            {
                clueObject.DOMoveY(moveObjectByY, 1f).SetEase(Ease.OutBounce);
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
        }

        public void RegisterClueListener()
        {
            clueRegistry.RegisterListener(this);
        }
    }
}
