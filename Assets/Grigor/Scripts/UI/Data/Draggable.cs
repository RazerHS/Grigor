using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Grigor.UI.Data
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image imageToDrag;

        public event Action<PointerEventData> OnBeginDragEvent;
        public event Action<PointerEventData> OnDragEvent;
        public event Action<PointerEventData> OnEndDragEvent;

        public void OnBeginDrag(PointerEventData eventData)
        {
            imageToDrag.raycastTarget = false;

            OnBeginDragEvent?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            imageToDrag.raycastTarget = true;

            OnEndDragEvent?.Invoke(eventData);
        }
    }
}
