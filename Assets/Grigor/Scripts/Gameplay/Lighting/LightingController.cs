using CardboardCore.DI;
using CardboardCore.Utilities;
using DG.Tweening;
using Grigor.Data;
using Grigor.Gameplay.Time;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    [Injectable]
    public class LightingController : CardboardCoreBehaviour
    {
        [SerializeField] private Light directionalLight;

        [PropertyTooltip("Offset of the sun rotation in degrees. This is used to make the sun rotation make more sense with the hours of day and night.")]
        [SerializeField] private float sunRotationDegreeOffset;

        [Inject] private TimeManager timeManager;

        private SceneConfig sceneConfig;
        private float currentSunRotation;

        public float CurrentSunRotation => currentSunRotation;

        protected override void OnInjected()
        {
            timeManager.TimeChangedEvent += OnTimeChanged;

            sceneConfig = SceneConfig.Instance;
        }

        protected override void OnReleased()
        {
            timeManager.TimeChangedEvent -= OnTimeChanged;
        }

        private void OnTimeChanged(int minutes, int hours)
        {
            UpdateLighting(timeManager.GetCurrentDayPercentage());
        }

        private void UpdateLighting(float timePercent)
        {
            if (directionalLight == null)
            {
                throw Log.Exception("No directional light set!");
            }

            RenderSettings.ambientLight = sceneConfig.AmbientColor.Evaluate(timePercent);

            directionalLight.color = sceneConfig.DirectionalColor.Evaluate(timePercent);

            Vector3 newSunRotation = new Vector3((timePercent * 360f) + sunRotationDegreeOffset - 90f, 170f, 0f);

            float transitionTime = sceneConfig.SmoothSunTransition ? sceneConfig.SmoothSunTransitionTime : 0f;

            directionalLight.transform.DOLocalRotate(newSunRotation, transitionTime).SetEase(Ease.OutSine);

            currentSunRotation = newSunRotation.x;
        }
    }
}
