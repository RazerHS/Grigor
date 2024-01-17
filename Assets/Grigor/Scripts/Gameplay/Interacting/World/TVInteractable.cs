﻿using CardboardCore.Utilities;
using Grigor.Gameplay.Interacting.Components;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

namespace Grigor.Gameplay.World.Components
{
    public class TVInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("TV References", false, true)] private VideoPlayer videoPlayer;
        [SerializeField, ColoredBoxGroup("TV References")] private Renderer screenRenderer;
        [SerializeField, ColoredBoxGroup("TV References")] private Renderer bleepButtonRenderer;
        [SerializeField, ColoredBoxGroup("TV References")] private Material screenOnReferenceMaterial;
        [SerializeField, ColoredBoxGroup("TV References")] private Material bleepButtonReferenceMaterial;
        [SerializeField, ColoredBoxGroup("TV References")] private Material screenOffMaterial;
        [SerializeField, ColoredBoxGroup("TV References"), Range(1, 4)] private int renderTextureDownscale = 2;

        [SerializeField, ColoredBoxGroup("TV Color and View", false, true)] private Color bleepButtonOnColor;
        [SerializeField, ColoredBoxGroup("TV Color and View")] private Color bleepButtonOffColor;
        [SerializeField, ColoredBoxGroup("TV Color and View"), Range(0f, 50f)] private float bleepButtonIntensity;
        [SerializeField, ColoredBoxGroup("TV Color and View")] private VideoClip videoToPlay;

        [ShowInInspector, ColoredBoxGroup("Debug"), ReadOnly] private bool screenActive;

        private RenderTexture renderTexture;
        private Material screenMaterial;
        private Material bleepButtonMaterial;

        private static readonly int EmissiveColor = Shader.PropertyToID("_EmissiveColor");

        protected override void OnInitialized()
        {
            if (videoToPlay == null)
            {
                throw Log.Exception($"No video clip assigned to {name}!");
            }

            renderTexture = new RenderTexture(1920 / renderTextureDownscale, 1080 / renderTextureDownscale, 24)
            {
                useDynamicScale = true
            };

            screenMaterial = new Material(screenOnReferenceMaterial) {mainTexture = renderTexture};
            bleepButtonMaterial = new Material(bleepButtonReferenceMaterial);

            bleepButtonRenderer.material = bleepButtonMaterial;

            videoPlayer.targetTexture = renderTexture;
            videoPlayer.clip = videoToPlay;

            TurnScreenOff();
        }

        protected override void OnInteractEffect()
        {
            screenActive = !screenActive;

            if (screenActive)
            {
                TurnScreenOn();
            }
            else
            {
                TurnScreenOff();
            }

            EndInteract();
        }

        private void TurnScreenOn()
        {
            videoPlayer.Play();

            float factor = Mathf.Pow(2f, bleepButtonIntensity);

            bleepButtonMaterial.SetColor(EmissiveColor, bleepButtonOnColor * factor);

            screenRenderer.material = screenMaterial;
        }

        private void TurnScreenOff()
        {
            videoPlayer.Stop();

            float factor = Mathf.Pow(2f, bleepButtonIntensity);

            bleepButtonMaterial.SetColor(EmissiveColor, bleepButtonOffColor * factor);

            screenRenderer.material = screenOffMaterial;
        }
    }
}