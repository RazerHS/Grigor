using CardboardCore.Utilities;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class DestroyObjectInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Destroying", false, true)] private GameObject objectToDestroy;
        [SerializeField, ColoredBoxGroup("Destroying")] private float delay;

        protected override void OnInteractEffect()
        {
            Helper.Delay(delay, DestroyObject);

            EndInteract();
        }

        private void DestroyObject()
        {
            if (objectToDestroy == null)
            {
                return;
            }

            Destroy(objectToDestroy);

            Log.Write($"Object <b>{objectToDestroy.name}</b> destroyed!");
        }
    }
}
