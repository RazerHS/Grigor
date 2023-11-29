using System;
using CardboardCore.Utilities;
using Grigor.Data.Clues;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        public ClueData ClueData => clueData;

        public event Action DragEndedEvent;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            draggable.OnBeginDragEvent += OnBeginDrag;
            draggable.OnDragEvent += OnDrag;
            draggable.OnEndDragEvent += OnEndDrag;
        }

        private void OnDisable()
        {
            draggable.OnBeginDragEvent -= OnBeginDrag;
            draggable.OnDragEvent -= OnDrag;
            draggable.OnEndDragEvent -= OnEndDrag;
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            if (isAttached)
            {
                DragEndedEvent?.Invoke();

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

        public void FlagAsAttached()
        {
            isAttached = true;
        }

        public void FlagAsDetached()
        {
            isAttached = false;
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
