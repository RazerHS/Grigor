using System.Collections.Generic;
using System.Linq;
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
            particleCulling.Initialize();
            particleCulling.SetRainZoneManager(rainZoneManager);

            particleCulling.CullEvent += OnCull;

            GetRenderers();

            DisableRain();
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
