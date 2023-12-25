using System.Collections.Generic;
using System.Linq;
using Codice.CM.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Weather
{
    public class RainZone : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> rainParticleSystems;
        [SerializeField] private List<Renderer> rainParticleRenderers;
        [SerializeField] private List<GameObject> particleSystemObjects;
        [SerializeField] private RainZoneCustomParticleCulling particleCulling;

        public RainZoneCustomParticleCulling ParticleCulling => particleCulling;
        private RainZoneManager rainZoneManager;

        [Button(ButtonSizes.Large)]
        private void GetRainParticleSystems()
        {
            rainParticleSystems.Clear();
            rainParticleRenderers.Clear();
            particleSystemObjects.Clear();

            rainParticleSystems = GetComponentsInChildren<ParticleSystem>().ToList();

            for (int i = 0; i < rainParticleSystems.Count; i++)
            {
                particleSystemObjects.Add(rainParticleSystems[i].gameObject);
                rainParticleRenderers.Add(rainParticleSystems[i].GetComponent<Renderer>());
            }
        }

        public float GetZoneSize()
        {
            return rainParticleSystems[0].shape.scale.x * rainParticleSystems.Count / 2;
        }

        private void GetRenderers()
        {
            for ( int i = 0; i < rainParticleSystems.Count; i++)
            {
                rainParticleRenderers.Add(rainParticleSystems[i].GetComponent<Renderer>());
            }
        }

        public void Initialize(RainZoneManager rainZoneManager)
        {
            this.rainZoneManager = rainZoneManager;

            particleCulling.Initialize();
            particleCulling.SetRainZoneManager(rainZoneManager);

            particleCulling.CullEvent += OnCull;

            rainZoneManager.SetRainParticleEmissionEvent += OnSetRainParticleEmission;

            GetRenderers();

            DisableRain();
        }

        public void Dispose()
        {
            particleCulling.CullEvent -= OnCull;

            rainZoneManager.SetRainParticleEmissionEvent -= OnSetRainParticleEmission;

            particleCulling.Dispose();
        }

        private void OnSetRainParticleEmission(float value)
        {
            for (int i = 0; i < rainParticleSystems.Count; i++)
            {
                ParticleSystem.EmissionModule emissionModule = rainParticleSystems[i].emission;
                emissionModule.rateOverTime = value;
            }
        }

        private void OnCull(bool visible)
        {
            if (visible)
            {
                EnableRain();
            }
            else
            {
                DisableRain();
            }
        }

        private void EnableRain()
        {
            for (int i = 0; i < rainParticleSystems.Count; i++)
            {
                rainParticleSystems[i].Play();
                rainParticleRenderers[i].enabled = true;
                particleSystemObjects[i].SetActive(true);

                ParticleSystem.EmissionModule emissionModule = rainParticleSystems[i].emission;
                emissionModule.enabled = true;
            }
        }

        public void DisableRain()
        {
            for (int i = 0; i < rainParticleSystems.Count; i++)
            {
                rainParticleSystems[i].Stop();
                rainParticleRenderers[i].enabled = false;
                particleSystemObjects[i].SetActive(false);

                ParticleSystem.EmissionModule emissionModule = rainParticleSystems[i].emission;
                emissionModule.enabled = false;
            }
        }
    }
}
