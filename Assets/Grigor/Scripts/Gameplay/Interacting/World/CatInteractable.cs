using System.Collections.Generic;
using DG.Tweening;
using Grigor.Gameplay.Interacting.Components;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class CatInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Meow", false, true)] private Transform catTransform;
        [SerializeField, ColoredBoxGroup("Meow")] private float timeItTakesToRoll;
        [SerializeField, ColoredBoxGroup("Meow")] private float rollDuration;
        [SerializeField, ColoredBoxGroup("Meow")] private float moveCatY;
        [SerializeField, ColoredBoxGroup("Meow")] private float rotateCatZ;
        [SerializeField, ColoredBoxGroup("Meow")] private float rotateCatY;
        [SerializeField, ColoredBoxGroup("Meow")] private List<Transform> wheelTransforms;

        private bool isRolling;

        protected override void OnInteractEffect()
        {
            if (isRolling)
            {
                EndInteract();

                return;
            }

            RollOver();

            StartSpinningWheels();

            Helper.Delay(rollDuration, GetUp);

            EndInteract();
        }

        private void GetUp()
        {
            isRolling = false;

            Vector3 newRotation = new Vector3(0, 0, 0);

            catTransform.DOLocalRotate(newRotation, timeItTakesToRoll).SetEase(Ease.InOutQuad);

            catTransform.DOLocalMoveY(catTransform.localPosition.y - moveCatY, timeItTakesToRoll).SetEase(Ease.InOutQuint);

            StopSpinningWheels();
        }

        private void RollOver()
        {
            isRolling = true;

            float zRotation = rotateCatZ;
            float yRotation = rotateCatY;

            Vector3 newRotation = new Vector3(0, yRotation * MultiplyByRandomSign(), zRotation * MultiplyByRandomSign());

            catTransform.DOLocalRotate(newRotation, timeItTakesToRoll).SetEase(Ease.InOutQuint);

            catTransform.DOLocalMoveY(catTransform.localPosition.y + moveCatY, timeItTakesToRoll).SetEase(Ease.InOutQuint);
        }

        private int MultiplyByRandomSign()
        {
            int random = Random.Range(0, 1);

            if (random == 0)
            {
                return -1;
            }

            return 1;
        }

        private void StartSpinningWheels()
        {
            foreach (Transform wheelTransform in wheelTransforms)
            {
                wheelTransform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            }
        }

        private void StopSpinningWheels()
        {
            foreach (Transform wheelTransform in wheelTransforms)
            {
                wheelTransform.DOKill();
            }
        }
    }
}
