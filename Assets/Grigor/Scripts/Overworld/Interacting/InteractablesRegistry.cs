using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Overworld.Interacting
{
    [Injectable]
    public class InteractablesRegistry : MonoBehaviour
    {
        [ShowInInspector, InlineEditor] private readonly List<Interactable> interactables = new();

        public void Register(Interactable interactable)
        {
            if (!interactables.Contains(interactable))
            {
                interactables.Add(interactable);
            }
        }

        public void Unregister(Interactable interactable)
        {
            if (interactables.Contains(interactable))
            {
                interactables.Remove(interactable);
            }
        }

        public bool TryGetNearestInRange(Vector3 point, out Interactable nearest)
        {
            float minDistance = float.MaxValue;
            nearest = null;

            List<Interactable> interactablesInRange = interactables.Where(interactable => interactable.IsInteractableInRange(point)).ToList();

            foreach (Interactable interactable in interactablesInRange)
            {
                float distance = Vector3.Distance(interactable.InteractPoint.position, point);

                if (!(distance < minDistance))
                {
                    continue;
                }

                minDistance = distance;
                nearest = interactable;
            }

            return nearest != null;
        }

        public void EnableInteractables()
        {
            foreach (Interactable interactable in interactables)
            {
                interactable.EnableInteractable();
            }
        }
    }
}
