using CardboardCore.Utilities;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class CreateObjectInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Creating", false, true)] private GameObject objectToCreate;
        [SerializeField, ColoredBoxGroup("Creating")] private Transform objectParent;
        [SerializeField, ColoredBoxGroup("Creating")] private float delay;

        protected override void OnInitialized()
        {
            if (objectToCreate == null)
            {
                throw Log.Exception($"Object to create not set in interactable <b>{name}</b>!");
            }

            if (objectParent == null)
            {
                throw Log.Exception($"Object parent not set in interactable <b>{name}</b>!");
            }
        }

        protected override void OnInteractEffect()
        {
            Helper.Delay(delay, InstantiateObject);

            EndInteract();
        }

        private void InstantiateObject()
        {
            Instantiate(objectToCreate, objectParent);

            Log.Write($"Object <b>{objectToCreate.name}</b> created!");
        }
    }
}
