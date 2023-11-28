using System;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Interacting.Components;
using Grigor.UI;
using Grigor.UI.Widgets;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting
{
    public class Interactable : CardboardCoreBehaviour
    {
        [SerializeField] private Transform interactPoint;
        [SerializeField] private float interactDistance = 3f;
        [SerializeField] private bool isBox;

        [SerializeField, ColoredBoxGroup("Debugging", false, 0.5f, 0.1f, 0.2f)] private bool interactingEnabled;

        [SerializeField, HideLabel] private InteractablesChain interactablesChain;

        [Inject] private InteractablesRegistry interactablesRegistry;
        [Inject] private UIManager uiManager;

        private bool inRange;
        private InteractWidget interactWidget;

        public Transform InteractPoint => interactPoint;
        public float InteractDistance => interactDistance;
        public bool InteractingEnabled => interactingEnabled;
        public InteractablesChain InteractablesChain => interactablesChain;

        public event Action InteractEvent;
        public event Action<Characters.Components.Character> InRangeEvent;
        public event Action<Characters.Components.Character> OutOfRangeEvent;

        [Button]
        private void ResetChain()
        {
            interactablesChain.ResetInitialChain(GetComponents<InteractableComponent>().ToList());
        }

        private void OnDrawGizmos()
        {
            if (!interactPoint)
            {
                Log.Error($"No interact point set for {name}!");
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(interactPoint.position, 0.15f);

            if (isBox)
            {
                Gizmos.DrawWireCube(interactPoint.position, Vector3.one * interactDistance);
            }
            else
            {
                Gizmos.DrawWireSphere(interactPoint.position, interactDistance);
            }
        }

        protected override void OnInjected()
        {
            interactablesRegistry.Register(this);

            interactablesChain.ResetInitialChain(GetComponents<InteractableComponent>().ToList());

            interactablesChain.OrderChain();

            interactWidget = uiManager.GetWidget<InteractWidget>();
        }

        protected override void OnReleased()
        {
            interactablesRegistry.Unregister(this);
        }

        private bool IsInteractableInRangeOfSphere(Vector3 point)
        {
            float distance = Vector3.Distance(point, interactPoint.position);

            return distance <= interactDistance;
        }

        private bool IsInteractableInRangeOfBox(Vector3 point)
        {
            if (!(point.x > interactPoint.position.x - interactDistance / 2 &&
                  point.x < interactPoint.position.x + interactDistance / 2))
            {
                return false;
            }

            if (!(point.z > interactPoint.position.z - interactDistance / 2 &&
                  point.z < interactPoint.position.z + interactDistance / 2))
            {
                return false;
            }

            return true;
        }

        public bool IsInteractableInRange(Vector3 point)
        {
            bool inRange = isBox ? IsInteractableInRangeOfBox(point) : IsInteractableInRangeOfSphere(point);

            return inRange;
        }

        public void EnableProximityEffect(Characters.Components.Character interactingCharacter)
        {
            if (inRange)
            {
                return;
            }

            inRange = true;

            interactWidget.Show();

            InRangeEvent?.Invoke(interactingCharacter);
        }

        public void DisableProximityEffect(Characters.Components.Character interactingCharacter)
        {
            if (!inRange)
            {
                return;
            }

            inRange = false;

            interactWidget.Hide();

            OutOfRangeEvent?.Invoke(interactingCharacter);
        }

        public void Interact(Characters.Components.Character interactingCharacter)
        {
            InteractEvent?.Invoke();
        }

        public void EnableInteractable()
        {
            interactingEnabled = true;

            interactablesChain.Initialize(this);
        }

        public void DisableInteractable()
        {
            interactingEnabled = false;

            interactablesChain.Dispose();
        }

        public void ResetInteractable()
        {
            interactingEnabled = true;
        }
    }
}
