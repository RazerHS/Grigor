using System.Collections.Generic;
using Grigor.Gameplay.Interacting.Components;
using Grigor.Gameplay.Objects;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class SodaMachineInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("References", false, true)] private Transform sodaSpawnPointTransform;
        [SerializeField, ColoredBoxGroup("References")] private Transform sodaLaunchTargetTransform;
        [SerializeField, ColoredBoxGroup("References")] private Transform sodaParent;
        [SerializeField, ColoredBoxGroup("References")] private SodaCan sodaCanPrefab;
        [SerializeField, ColoredBoxGroup("References")] private Material sodaCanMaterial;

        [SerializeField, ColoredBoxGroup("Soda Launching", false, true), Range(0, 20)] private int maxSodas;
        [SerializeField, ColoredBoxGroup("Soda Launching"), Range(0f, 10f)] private float machineCooldown;
        [SerializeField, ColoredBoxGroup("Soda Launching"), Range(0f, 20f)] private float launchForce;
        [SerializeField, ColoredBoxGroup("Soda Launching"), Range(0f, 20f)] private float torqueForce;

        [ShowInInspector, ColoredBoxGroup("Debug"), ReadOnly] private bool onCooldown;

        private readonly Queue<SodaCan> currentSodaCans = new();

        protected override void OnInteractEffect()
        {
            parentInteractable.PauseInteractable();

            SodaCan sodaCan = SpawnAndLaunchSodaCan();

            currentSodaCans.Enqueue(sodaCan);

            if (currentSodaCans.Count >= maxSodas)
            {
                SodaCan sodaToDestroy = currentSodaCans.Dequeue();

                Destroy(sodaToDestroy.gameObject);
            }

            Helper.Delay(machineCooldown, EndCooldown);

            EndInteract();
        }

        private SodaCan SpawnAndLaunchSodaCan()
        {
            SodaCan sodaCan = Instantiate(sodaCanPrefab, sodaSpawnPointTransform.position, Quaternion.identity);

            sodaCan.transform.SetParent(sodaParent);

            sodaCan.Rigidbody.isKinematic = false;

            sodaCan.Renderer.material = sodaCanMaterial;

            Vector3 launchDirection = (sodaLaunchTargetTransform.position - sodaSpawnPointTransform.position).normalized;

            sodaCan.Rigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            sodaCan.Rigidbody.AddTorque(Vector3.up * torqueForce, ForceMode.Impulse);

            return sodaCan;
        }

        private void EndCooldown()
        {
            parentInteractable.UnpauseInteractable();
        }
    }
}
