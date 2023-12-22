using CardboardCore.DI;
using CardboardCore.Utilities;
using DG.Tweening;
using Grigor.Data;
using Grigor.Gameplay.Time;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    public class LightingController : CardboardCoreBehaviour
    {
        [SerializeField] private Light directionalLight;

        [PropertyTooltip("Offset of the sun rotation in degrees. This is used to make the sun rotation make more sense with the hours of day and night.")]
        [SerializeField] private float sunRotationDegreeOffset;

        [Inject] private TimeManager timeManager;

        protected override void OnInjected()
        {
            timeManager.TimeChangedEvent += OnTimeChanged;
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

            RenderSettings.ambientLight = GameConfig.Instance.AmbientColor.Evaluate(timePercent);
            RenderSettings.fogColor = GameConfig.Instance.FogColor.Evaluate(timePercent);

            if (directionalLight == null)
            {
                return;
            }

            directionalLight.color = GameConfig.Instance.DirectionalColor.Evaluate(timePercent);

            Vector3 newRotation = new Vector3((timePercent * 360f) + sunRotationDegreeOffset - 90f, 170f, 0f);
            directionalLight.transform.DOLocalRotate(newRotation, 0.5f).SetEase(Ease.OutSine);
        }
    }
}
