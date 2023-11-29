using System;
using CardboardCore.DI;
using Grigor.Data.Credentials;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Grigor.UI.Data
{
    [Serializable]
    public class CredentialUIDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI credentialText;
        [SerializeField] private Droppable clueDroppable;
        [SerializeField] private RectTransform clueHolderTransform;

        [ShowInInspector] private ClueUIDisplay attachedClueUIDisplay;

        private CredentialType credentialType;
        private bool ignoreFirstEndDrag = true;

        public CredentialType CredentialType => credentialType;
        public ClueUIDisplay AttachedClueUIDisplay => attachedClueUIDisplay;
        public RectTransform ClueHolderTransform => clueHolderTransform;

        public event Action<CredentialUIDisplay, ClueUIDisplay> CheckDroppedClueEvent;

        private void Awake()
        {
            clueDroppable.OnDropEvent += OnDrop;
        }

        private void OnDisable()
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
    }
}
