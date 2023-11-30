using System;
using Grigor.Data.Clues;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Grigor.UI.Data
{
    public class ClueUIDisplay : MonoBehaviour
    {
        [SerializeField] private Draggable draggable;
        [SerializeField] private RectTransform viewTransform;
        [SerializeField] private TextMeshProUGUI clueText;

        private RectTransform rectTransform;
        private ClueData clueData;
        private Vector3 defaultPosition;
        private bool isAttached;
        private CredentialUIDisplay attachedToCredentialUIDisplay;
        private bool ignoreFirstEndDrag;

        public ClueData ClueData => clueData;
        public CredentialUIDisplay AttachedToCredentialUIDisplay => attachedToCredentialUIDisplay;

        public event Action<ClueUIDisplay> ClueDragStartedEvent;

        public void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();

            draggable.OnBeginDragEvent += OnBeginDrag;
            draggable.OnDragEvent += OnDrag;
            draggable.OnEndDragEvent += OnEndDrag;
        }

        public void Dispose()
        {
            draggable.OnBeginDragEvent -= OnBeginDrag;
            draggable.OnDragEvent -= OnDrag;
            draggable.OnEndDragEvent -= OnEndDrag;
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            if (isAttached)
            {
                return;
            }

            if (ignoreFirstEndDrag)
            {
                ignoreFirstEndDrag = false;
                return;
            }

            viewTransform.anchoredPosition = defaultPosition;
        }

        private void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(viewTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldDragPoint);

            viewTransform.position = worldDragPoint;
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            ClueDragStartedEvent?.Invoke(this);

            defaultPosition = viewTransform.anchoredPosition;
        }

        public void DisableDrag()
        {
            draggable.enabled = false;
        }

        public void SetClueText(string text)
        {
            clueText.text = text;
        }

        public void SetClueData(ClueData clueData)
        {
            this.clueData = clueData;
        }

        public void FlagAsAttached(CredentialUIDisplay credentialUIDisplay)
        {
            ignoreFirstEndDrag = true;

            isAttached = true;
            attachedToCredentialUIDisplay = credentialUIDisplay;
        }

        public void FlagAsDetached()
        {
            ignoreFirstEndDrag = false;

            isAttached = false;
            attachedToCredentialUIDisplay = null;
        }

        public void ResetPosition()
        {
            viewTransform.anchoredPosition = Vector2.zero;
        }

        public void MoveParentRectTransformTo(Vector3 position)
        {
            rectTransform.position = position;
        }
    }
}
