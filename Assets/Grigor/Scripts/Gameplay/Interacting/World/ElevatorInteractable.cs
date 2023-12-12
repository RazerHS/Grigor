using System;
using DG.Tweening;
using Grigor.Gameplay.Interacting.Components;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class ElevatorInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Elevator", false, true)] private Transform elevatorTransform;
        [SerializeField, ColoredBoxGroup("Elevator")] private float elevatorDuration;
        [SerializeField, ColoredBoxGroup("Elevator")] private float elevatorTargetPositionY;

        private float elevatorOriginPositionY;
        private bool elevatorAtTarget;

        protected override void OnInitialized()
        {
            elevatorOriginPositionY = elevatorTransform.localPosition.y;
        }

        protected override void OnInteractEffect()
        {
            Vector3 targetPosition = elevatorTransform.localPosition;

            targetPosition.y = elevatorAtTarget ? elevatorOriginPositionY : elevatorTargetPositionY;

            elevatorTransform.DOLocalMove(targetPosition, elevatorDuration).SetEase(Ease.OutSine).OnComplete(OnElevatorArrived);
        }

        private void OnElevatorArrived()
        {
            elevatorAtTarget = !elevatorAtTarget;

            EndInteract();
        }
    }
}
