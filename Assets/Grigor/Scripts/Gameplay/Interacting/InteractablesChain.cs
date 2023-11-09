using System;
using System.Collections.Generic;
using System.Linq;
using CardboardCore.Utilities;
using Grigor.Gameplay.Interacting.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting
{
    [Serializable]
    public class InteractablesChain
    {
        [SerializeField, LabelText("Chain")]
        private List<InteractableComponent> interactableComponents = new();

        public int InteractablesInChain => interactableComponents.Count;

        public void OrderChain()
        {
            interactableComponents = interactableComponents.OrderBy(interactableComponent => interactableComponent.IndexInChain).ToList();
        }

        public void AddToChain(InteractableComponent interactableComponent)
        {
            if (interactableComponents.Contains(interactableComponent))
            {
                return;
            }

            interactableComponents.Add(interactableComponent);

            OrderChain();
        }

        public bool TryRemoveFromChain(InteractableComponent interactableComponent)
        {
            if (!interactableComponents.Contains(interactableComponent))
            {
                return false;
            }

            if (!interactableComponent.RemoveFromChainAfterEffect)
            {
                return false;
            }

            interactableComponents.Remove(interactableComponent);

            OrderChain();

            return true;
        }

        public void ResetChain(List<InteractableComponent> interactableComponents)
        {
            this.interactableComponents = interactableComponents;

            OrderChain();
        }

        public bool TryGetNextInChainAfter(InteractableComponent interactableComponent, out InteractableComponent nextInChain)
        {
            nextInChain = null;

            if (!interactableComponents.Contains(interactableComponent))
            {
                throw Log.Exception($"Interactable component {interactableComponent.name} is not in chain!");
            }

            int index = interactableComponents.IndexOf(interactableComponent);

            if (index >= interactableComponents.Count - 1)
            {
                Log.Write("No next in chain!");
                return false;
            }

            nextInChain = interactableComponents[index + 1];

            return true;
        }

        public void Initialize(Interactable interactable)
        {
            interactableComponents.ForEach(interactableComponent => interactableComponent.Initialize());

            if (InteractablesInChain == 0)
            {
                Log.Write($"No interactables in chain in {interactable.name}!");
                return;
            }

            interactableComponents[0].EnableInteraction();
        }

        public void Dispose()
        {
            interactableComponents.ForEach(interactableComponent => interactableComponent.Dispose());
        }

        public bool TryGetNextInChain(out InteractableComponent nextInChain, bool disableIfChainEnded = true)
        {
            nextInChain = null;

            if (InteractablesInChain == 0)
            {
                if (disableIfChainEnded)
                {
                    Log.Write("Chain ended!");
                }

                return false;
            }

            nextInChain = interactableComponents[0];

            return true;
        }
    }
}
