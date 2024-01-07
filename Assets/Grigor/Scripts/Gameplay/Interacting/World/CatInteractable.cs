using System.Collections.Generic;
using CardboardCore.DI;
using DG.Tweening;
using Grigor.Data;
using Grigor.Gameplay.Cats;
using Grigor.Gameplay.Interacting.Components;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
        [SerializeField, ColoredBoxGroup("Meow")] private float catSpeed;
        [SerializeField, ColoredBoxGroup("Meow")] private bool drawCatnipRadiusGizmo;
        [SerializeField, ColoredBoxGroup("Meow")] private List<Transform> wheelTransforms;
        [SerializeField, ColoredBoxGroup("Meow")] private NavMeshAgent navMeshAgent;

        [Inject] private CatManager catManager;

        private bool isRolling;
        private bool isMoving;
        private Transform currentCatnipTransform;
        private GameConfig gameConfig;

        private void OnDrawGizmos()
        {
            if (!drawCatnipRadiusGizmo)
            {
                return;
            }

            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position, GameConfig.Instance.CatnipRadius);
        }

        protected override void OnInitialized()
        {
            catManager.RegisterCat(this);

            gameConfig = GameConfig.Instance;
        }

        protected override void OnDisposed()
        {
            catManager.UnregisterCat(this);
        }

        private void Update()
        {
            if (currentCatnipTransform == null)
            {
                return;
            }

            if (isRolling)
            {
                return;
            }

            isMoving = IsCurrentCatnipInRange();

            if (navMeshAgent.isStopped || !isMoving)
            {
                return;
            }

            navMeshAgent.SetDestination(currentCatnipTransform.position);

            float distance = Vector3.Distance(navMeshAgent.destination, transform.position);

            if (!(distance <= navMeshAgent.stoppingDistance))
            {
                return;
            }

            currentCatnipTransform = null;

            isMoving = false;

            RollOver();
        }

        protected override void OnInteractEffect()
        {
            if (isRolling)
            {
                EndInteract();

                return;
            }

            RollOver();

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

            StartSpinningWheels();

            Helper.Delay(rollDuration, GetUp);
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

        public void OnCatnipPlaced(Transform targetCatnip)
        {
            currentCatnipTransform = targetCatnip;

            if (isRolling)
            {
                return;
            }

            StartSpinningWheels();
        }

        public void OnCatnipRemoved()
        {
            currentCatnipTransform = null;

            isMoving = false;
        }

        private bool IsCurrentCatnipInRange()
        {
            float distance = Vector3.Distance(currentCatnipTransform.position, transform.position);

            return distance <= gameConfig.CatnipRadius;
        }

    }
}
