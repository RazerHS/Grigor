﻿using System;
using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Interacting.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting
{
    public class Interactable : CardboardCoreBehaviour
    {
        [SerializeField] private Transform interactPoint;
        [SerializeField] private float interactDistance = 3f;
        [SerializeField] private bool isBox;

        [SerializeField, HideInInspector] private bool interactingEnabled;

        [SerializeField] private List<InteractableComponent> interactableComponents;

        [Inject] private InteractablesRegistry interactablesRegistry;

        private bool inRange;

        public Transform InteractPoint => interactPoint;
        public float InteractDistance => interactDistance;
        public List<InteractableComponent> InteractableComponents => interactableComponents;
        public bool InteractingEnabled => interactingEnabled;

        public event Action InteractEvent;
        public event Action<Characters.Components.Character> InRangeEvent;
        public event Action<Characters.Components.Character> OutOfRangeEvent;

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

        [OnInspectorInit]
        private void GetAllInteractableComponents()
        {
            interactableComponents = GetComponents<InteractableComponent>().ToList();
        }

        protected override void OnInjected()
        {
            interactablesRegistry.Register(this);

            if (interactableComponents.Count == 0)
            {
                GetAllInteractableComponents();
            }
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

            Log.Write($"In range of interactable <b>{name}</b>");

            InRangeEvent?.Invoke(interactingCharacter);
        }

        public void DisableProximityEffect(Characters.Components.Character interactingCharacter)
        {
            if (!inRange)
            {
                return;
            }

            Log.Write($"Out of range of interactable <b>{name}</b>");

            inRange = false;

            OutOfRangeEvent?.Invoke(interactingCharacter);
        }

        public void Interact(Characters.Components.Character interactingCharacter)
        {
            InteractEvent?.Invoke();
        }

        public InteractableComponent GetPrimaryInteractable()
        {
            // TO-DO: create flexible chains
            if (interactableComponents.Count == 0)
            {
                throw Log.Exception($"There are no interactable components on this {name}!");
            }

            return interactableComponents[0];
        }

        public void EnableInteractable()
        {
            interactingEnabled = true;

            interactableComponents.ForEach(interactableComponent => interactableComponent.Initialize());
        }

        public void DisableInteractable()
        {
            interactingEnabled = false;

            interactableComponents.ForEach(interactableComponent => interactableComponent.Dispose());
        }
    }
}