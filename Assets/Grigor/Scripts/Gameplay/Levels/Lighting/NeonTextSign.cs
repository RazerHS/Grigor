using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    public class NeonTextSign : MonoBehaviour
    {
        [SerializeField, ColoredBoxGroup("Neon", false, true), OnValueChanged(nameof(ChangeColor))] private Color emissiveColor;
        [SerializeField, ColoredBoxGroup("Neon")] private Renderer emissiveRenderer;
        [SerializeField, ColoredBoxGroup("Neon"), Range(0f, 20f), OnValueChanged(nameof(ChangeColor))] private float intensity;

        private static readonly int EmissionColor = Shader.PropertyToID("_GlowColor");

        private void Awake()
        {
            Material newMaterial = new Material(emissiveRenderer.material);

            emissiveRenderer.material = newMaterial;

            ChangeColor();
        }

        private void ChangeColor()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            float factor = Mathf.Pow(2f, intensity);

            emissiveRenderer.material.SetColor(EmissionColor, emissiveColor * factor);
        }
    }
}
