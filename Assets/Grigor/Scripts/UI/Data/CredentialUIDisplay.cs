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
    public class CredentialUIDisplay : CardboardCoreBehaviour
    {
        [SerializeField] private TextMeshProUGUI credentialText;
        [SerializeField] private Droppable clueDroppable;
        [SerializeField] private RectTransform clueHolderTransform;

        [Inject] private UIManager uiManager;

        [ShowInInspector] private ClueUIDisplay attachedClueUIDisplay;

        private CredentialType credentialType;
        private DataPodWidget dataPodWidget;
        private bool ignoreFirstEndDrag = true;

        public CredentialType CredentialType => credentialType;
        public ClueUIDisplay AttachedClueUIDisplay => attachedClueUIDisplay;
        public RectTransform ClueHolderTransform => clueHolderTransform;

        public event Action<CredentialUIDisplay, ClueUIDisplay> CheckDroppedClueEvent;

        protected override void OnInjected()
        {
            dataPodWidget = uiManager.GetWidget<DataPodWidget>();

            clueDroppable.OnDropEvent += OnDrop;
        }

        protected override void OnReleased()
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

            attachedClueUIDisplay.DragEndedEvent += OnAttachedClueDisplayDragEnded;

            attachedClueUIDisplay.FlagAsAttached();
            attachedClueUIDisplay.transform.SetParent(clueHolderTransform);

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

        private void OnAttachedClueDisplayDragEnded()
        {
            //a hack to prevent the first end drag event from being fired when the clue is dropped on the credential
            if (ignoreFirstEndDrag)
            {
                ignoreFirstEndDrag = false;
                return;
            }

            attachedClueUIDisplay.DragEndedEvent -= OnAttachedClueDisplayDragEnded;

            attachedClueUIDisplay.FlagAsDetached();
            attachedClueUIDisplay.transform.SetParent(dataPodWidget.ClueDisplayParent);
            attachedClueUIDisplay.ResetPosition();

            attachedClueUIDisplay = null;

            ignoreFirstEndDrag = true;
        }
    }
}
