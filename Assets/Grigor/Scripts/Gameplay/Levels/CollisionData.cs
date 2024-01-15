using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Time
{
    public class CollisionData : MonoBehaviour
    {
        [SerializeField] private bool useLayerMask;
        [SerializeField, ShowIf(nameof(useLayerMask))] private LayerMask layerMask;

        public event Action<Collision> CollisionEvent;
        public event Action MouseEnterEvent;
        public event Action MouseExitEvent;
        public event Action MouseDownEvent;

        private void OnCollisionEnter(Collision collision)
        {
            if (useLayerMask)
            {
                if ((layerMask & (1 << collision.gameObject.layer)) == 0)
                {
                    return;
                }
            }

            CollisionEvent?.Invoke(collision);
        }

        private void OnMouseEnter()
        {
            MouseEnterEvent?.Invoke();
        }

        private void OnMouseExit()
        {
            MouseExitEvent?.Invoke();
        }

        private void OnMouseDown()
        {
            MouseDownEvent?.Invoke();
        }
    }
}
