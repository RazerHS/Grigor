using CardboardCore.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Overworld.Lighting
{
    public class LightingManager : MonoBehaviour
    {
        [SerializeField] private Light directionalLight;
        [SerializeField] private bool changeTimeAutomatically;
        [SerializeField, Range(0, 24), OnValueChanged("UpdateLighting")] private float timeOfDay;

        public float TimeOfDay => timeOfDay;

        private void Update()
        {
            UpdateLighting();

            if (!changeTimeAutomatically)
            {
                return;
            }

            timeOfDay += Time.deltaTime;
            timeOfDay %= 24;
        }

        private void UpdateLighting()
        {
            if (directionalLight == null)
            {
                throw Log.Exception("No directional light set!");
            }

            float timePercent = timeOfDay / 24f;

            RenderSettings.ambientLight = LightingConfig.Instance.AmbientColor.Evaluate(timePercent);
            RenderSettings.fogColor = LightingConfig.Instance.FogColor.Evaluate(timePercent);

            if (directionalLight == null)
            {
                return;
            }

            directionalLight.color = LightingConfig.Instance.DirectionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0f));
        }

        public float SetTimeOfDay(float timeOfDay)
        {
            this.timeOfDay = timeOfDay;
            this.timeOfDay %= 24;

            return timeOfDay;
        }
    }
}
