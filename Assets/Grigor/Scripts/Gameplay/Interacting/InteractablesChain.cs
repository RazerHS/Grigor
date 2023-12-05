using System;
using System.Collections.Generic;
using System.Linq;
using CardboardCore.Utilities;
using Grigor.Gameplay.Interacting.Components;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting
{
    [Serializable]
    public class InteractablesChain
    {
        [SerializeField, LabelText("Chain"), ListElementInlineButton(nameof(IncrementElementIndex), "", SdfIconType.CaretDownFill), ListElementInlineButton(nameof(DecrementElementIndex), "", SdfIconType.CaretUpFill)] private List<InteractableComponent> interactableComponents = new();

        public int InteractablesInChainCount => interactableComponents.Count;

        public void OrderChain()
        {
            interactableComponents = interactableComponents.OrderBy(interactableComponent => interactableComponent.IndexInChain).ToList();
        }

        private void RefreshChainIndeces()
        {
            for (int i = 0; i < interactableComponents.Count; i++)
            {
                interactableComponents[i].SetDefaultIndexInChain(i);
            }
        }

        private void IncrementElementIndex(int index, InteractableComponent element)
        {
            int targetIndex = index + 1;

            if (targetIndex >= interactableComponents.Count)
            {
                return;
            }

            InteractableComponent elementToSwap = interactableComponents[targetIndex];

            interactableComponents[targetIndex] = element;
            interactableComponents[index] = elementToSwap;

            RefreshChainIndeces();
        }

        private void DecrementElementIndex(int index, InteractableComponent element)
        {
            int targetIndex = index - 1;

            if (targetIndex < 0)
            {
                return;
            }

            InteractableComponent elementToSwap = interactableComponents[targetIndex];

            interactableComponents[targetIndex] = element;
            interactableComponents[index] = elementToSwap;

            RefreshChainIndeces();
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

        public void ResetInitialChain(List<InteractableComponent> interactableComponents)
        {
            this.interactableComponents = interactableComponents;

            OrderChain();

            RefreshChainIndeces();
        }

        private void ResetChain()
        {
            OrderChain();

            interactableComponents.ForEach(interactableComponent => interactableComponent.OnCurrentChainEnded());
        }

        private bool ContinueCurrentChain(InteractableComponent nextInChain)
        {
            if (!nextInChain.InteractedWithInCurrentChain)
            {
                return true;
            }

            Log.Write("Current chain ended and reset!");

            ResetChain();

            return false;
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
                Log.Write("Current chain ended!");

                return false;
            }

            nextInChain = interactableComponents[index + 1];

            return ContinueCurrentChain(nextInChain);
        }

        public void Initialize(Interactable interactable)
        {
            interactableComponents.ForEach(interactableComponent => interactableComponent.Initialize());

            if (InteractablesInChainCount == 0)
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

        public bool TryGetNextInChain(out InteractableComponent nextInChain, bool allowLogs = true)
        {
            nextInChain = null;

            if (InteractablesInChainCount == 0)
            {
                if (allowLogs)
                {
                    Log.Write("Chain has no more interactables!");
                }

                return false;
            }

            nextInChain = interactableComponents[0];

            return ContinueCurrentChain(nextInChain);
        }
    }
}
