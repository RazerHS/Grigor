using System;
using UnityEngine;

namespace Grigor.Gameplay.Weather
{
    public class RainZoneCustomParticleCulling : MonoBehaviour
    {
        [SerializeField, HideInInspector] private RainZoneManager rainZoneManager;

        private Camera currentCamera;

        private CullingGroup cullingGroup;

        public event Action<bool> CullEvent;

        private void OnDrawGizmos()
        {
            if (!rainZoneManager.DrawGizmos)
            {
                return;
            }

            if (cullingGroup == null)
            {
                return;
            }

            Color col = Color.yellow;

            if (cullingGroup != null && !cullingGroup.IsVisible(0))
            {
                col = Color.red;
            }

            Gizmos.color = col;
            Gizmos.DrawWireSphere(transform.position, rainZoneManager.CullingRadius);
        }

        public void SetRainZoneManager(RainZoneManager rainZoneManager)
        {
            this.rainZoneManager = rainZoneManager;
        }

        public void Initialize()
        {
            currentCamera = Camera.main;

            cullingGroup = new CullingGroup();
            cullingGroup.targetCamera = currentCamera;

            cullingGroup.SetBoundingSpheres(new[]
            {
                new BoundingSphere(transform.position, rainZoneManager.CullingRadius)
            });

            cullingGroup.SetBoundingSphereCount(1);

            Cull(cullingGroup.IsVisible(0));

            ResetBoundingDistance();
            cullingGroup.SetDistanceReferencePoint(currentCamera.transform);

            cullingGroup.enabled = true;

            cullingGroup.onStateChanged += OnStateChanged;
        }

        private void ResetBoundingDistance()
        {
            cullingGroup.SetBoundingDistances(new[] { rainZoneManager.CullingDistance });
        }

        public void Dispose()
        {
            cullingGroup.enabled = false;

            cullingGroup.onStateChanged -= OnStateChanged;

            cullingGroup?.Dispose();
        }

        private void OnStateChanged(CullingGroupEvent @event)
        {
            Cull(@event.isVisible);
        }

        private void Cull(bool visible)
        {
           CullEvent?.Invoke(visible);
        }
    }
}
