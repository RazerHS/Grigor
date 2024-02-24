using System.Collections.Generic;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    public class NeonLightGroup : MonoBehaviour
    {
        [SerializeField] private List<Renderer> renderers;
        [SerializeField] private bool permanentlyVisible;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissiveColor");
        private static readonly int EmissiveIntensity = Shader.PropertyToID("_EmissiveIntensity");

        public void Initialize(Color color, bool willDisappear)
        {
            if (!permanentlyVisible)
            {
                if (willDisappear)
                {
                    gameObject.SetActive(false);

                    return;
                }
            }

            renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());

            foreach (Renderer renderer in renderers)
            {
                Material material = renderer.material;

                float intensity = material.GetFloat(EmissiveIntensity);

                material.SetColor(EmissionColor, color * intensity);
            }
        }
    }
}
