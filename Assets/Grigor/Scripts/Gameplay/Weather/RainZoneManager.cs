using System;
using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Weather
{
    [Injectable]
    public class RainZoneManager : MonoBehaviour
    {
        [SerializeField] private RainZone rainZonePrefab;
        [SerializeField] private Transform rainParticleSystemParent;
        [SerializeField] private Vector2 rainZoneSize = new Vector2(1, 1);
        [SerializeField] private float cullingRadius = 10;
        [SerializeField] private float cullingDistance = 10f;
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private List<RainZone> rainZones = new List<RainZone>();

        public float CullingRadius => cullingRadius;
        public float CullingDistance => cullingDistance;
        public bool DrawGizmos => drawGizmos;

        public event Action<float> SetRainParticleEmissionEvent;
        public event Action<float, float, float> SetRainParticleRotationEvent;

        private void Awake()
        {
            foreach (RainZone rainZone in rainZones)
            {
                rainZone.Initialize(this);
            }
        }

        private void OnDestroy()
        {
            foreach (RainZone rainZone in rainZones)
            {
                rainZone.Dispose();
            }
        }

        [Button(ButtonSizes.Large)]
        private void SpawnRainZones()
        {
            ClearRainZones();

            if (rainZonePrefab == null)
            {
                throw Log.Exception("Rain particle system prefab not set!");
            }

            for (int i = 0; i < rainZoneSize.x; i++)
            {
                for (int j = 0; j < rainZoneSize.y; j++)
                {
                    RainZone rainZone = Instantiate(rainZonePrefab, rainParticleSystemParent, true);

                    rainZone.DisableRain();
                    rainZone.ParticleCulling.SetRainZoneManager(this);

                    rainZones.Add(rainZone);

                    float zoneSize = rainZone.GetZoneSize();
                    float offsetX = rainZoneSize.x / 2f;
                    float offsetZ = rainZoneSize.y / 2f;

                    rainZone.transform.position = new Vector3((i * zoneSize) - offsetX, 1, (j * zoneSize) - offsetZ) + transform.position;
                }
            }
        }

        private void ClearRainZones()
        {
            for (int i = rainZones.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(rainZones[i].gameObject);
            }

            rainZones.Clear();
        }

        public void SetRainParticleEmission(float value)
        {
            SetRainParticleEmissionEvent?.Invoke(value);
        }

        public void SetRainParticleRotation(float value, float windStrength, float maxAngle)
        {
            SetRainParticleRotationEvent?.Invoke(value, windStrength, maxAngle);
        }
    }
}
