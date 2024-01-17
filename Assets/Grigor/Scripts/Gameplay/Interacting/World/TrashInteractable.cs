using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Characters;
using Grigor.Gameplay.Interacting.Components;
using MEC;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class TrashInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Trash Launching", false, true)] private Transform trashContentsTransform;
        [SerializeField, ColoredBoxGroup("Trash Launching")] private float launchForce;
        [SerializeField, ColoredBoxGroup("Trash Launching")] private float upwardsLaunchForce;
        [SerializeField, ColoredBoxGroup("Trash Launching")] private Vector3 launchTorque;
        [SerializeField, ColoredBoxGroup("Trash Launching"), Range(0, 5f)] private float delayBetweenLaunches = 0.1f;

        private readonly List<Rigidbody> trashContents = new();

        [ShowInInspector, ColoredBoxGroup("Debug"), ReadOnly] private bool trashRummaged;

        [Inject] private CharacterRegistry characterRegistry;

        protected override void OnInitialized()
        {
            foreach (Transform child in trashContentsTransform)
            {
                Rigidbody newRigidbody = child.TryGetComponent(out Rigidbody rigidbody) ? rigidbody : child.gameObject.AddComponent<Rigidbody>();

                trashContents.Add(newRigidbody);
            }
        }

        protected override void OnInteractEffect()
        {
            if (trashRummaged)
            {
                EndInteract();

                return;
            }

            trashRummaged = true;

            Timing.RunCoroutine(LaunchTrashContents());

            EndInteract();
        }

        private IEnumerator<float> LaunchTrashContents()
        {
            Vector3 playerLookDirection = characterRegistry.Player.Look.LookTransform.forward.normalized;

            foreach (Rigidbody rigidbody in trashContents)
            {
                rigidbody.isKinematic = false;

                rigidbody.AddForce((launchForce * -playerLookDirection) + (Vector3.up * upwardsLaunchForce), ForceMode.Impulse);
                rigidbody.AddTorque(launchTorque, ForceMode.Impulse);

                yield return Timing.WaitForSeconds(delayBetweenLaunches);
            }
        }
    }
}
