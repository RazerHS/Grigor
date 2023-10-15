using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Time;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    public class LightingController : CardboardCoreBehaviour
    {
        [SerializeField] private Light directionalLight;

        [Inject] private TimeManager timeManager;

        protected override void OnInjected()
        {
            timeManager.TimeChangedEvent += OnTimeChanged;
        }

        protected override void OnReleased()
        {
            timeManager.TimeChangedEvent -= OnTimeChanged;
        }

        private void OnTimeChanged(float time)
        {
            UpdateLighting(time / 24f);
        }

        // TO-DO: make sun rotate all 360 degrees instead of back and forth
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
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0f));
        }
    }
}
