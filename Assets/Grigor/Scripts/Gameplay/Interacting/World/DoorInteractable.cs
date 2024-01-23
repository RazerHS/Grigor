using DG.Tweening;
using Grigor.Gameplay.Interacting.Components;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class DoorInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Door", false, true)] private Transform doorTransform;
        [SerializeField, ColoredBoxGroup("Door")] private float rotationAngleY = 90f;
        [SerializeField, ColoredBoxGroup("Door")] private bool rotateClockwise = true;
        [SerializeField, ColoredBoxGroup("Door")] private float rotationDuration = 1f;
        [SerializeField, ColoredBoxGroup("Door")] private bool locked;
        [SerializeField, ColoredBoxGroup("Door")] private bool isClosed;

        private Vector3 defaultRotation;

        protected override void OnInitialized()
        {
            defaultRotation = doorTransform.rotation.eulerAngles;
        }

        protected override void OnInteractEffect()
        {
            isClosed = !isClosed;

            if (isClosed)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }

            EndInteractEffect();
        }

        protected void OpenDoor()
        {
            if (locked)
            {
                return;
            }

            Vector3 angleToRotate = new Vector3(0f, rotationAngleY, 0f);

            if (rotateClockwise)
            {
                angleToRotate *= -1;
            }

            doorTransform.DORotate(angleToRotate, rotationDuration);
        }

        protected void CloseDoor()
        {
            doorTransform.DORotate(defaultRotation, rotationDuration);
        }

        public void UnlockDoor()
        {
            locked = false;
        }

        public void LockDoor()
        {
            locked = true;
        }
    }
}
