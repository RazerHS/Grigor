using System.Collections.Generic;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    public class NeonLightGroup : MonoBehaviour
    {
        [SerializeField] private List<Renderer> renderers;
        [SerializeField] private bool permanentlyVisible;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public void Initialize(Color color, bool willDisappear)
        {
            if (willDisappear || !permanentlyVisible)
            {
                gameObject.SetActive(false);

                return;
            }

            renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());

            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetColor(EmissionColor, color);
            }
        }
    }
}
