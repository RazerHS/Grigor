using CardboardCore.DI;
using CardboardCore.Utilities;
using DG.Tweening;
using Grigor.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Time.Lighting
{
    public class SunController : CardboardCoreBehaviour
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
            int totalDayMinutes = 60 * 24;
            float currentPassedMinutes = hours * 60 + minutes;

            UpdateSunPosition(currentPassedMinutes / totalDayMinutes);
        }

        // TO-DO: make sun rotate all 360 degrees instead of back and forth
        private void UpdateSunPosition(float timePercent)
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
