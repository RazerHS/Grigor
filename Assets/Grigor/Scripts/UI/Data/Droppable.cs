using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Grigor.UI.Data
{
    public class Droppable : MonoBehaviour, IDropHandler
    {
        public event Action<PointerEventData> OnDropEvent;

        public void OnDrop(PointerEventData eventData)
        {
            OnDropEvent?.Invoke(eventData);
        }
    }
}
