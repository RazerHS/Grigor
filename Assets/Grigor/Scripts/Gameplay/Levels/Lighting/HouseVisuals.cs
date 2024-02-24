using System.Collections.Generic;
using Grigor.Data;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    public class HouseVisuals : MonoBehaviour
    {
        [SerializeField] private List<NeonLightGroup> neonLightGroups;

        private SceneConfig sceneConfig;

        public void Awake()
        {
            sceneConfig = SceneConfig.Instance;

            neonLightGroups = new List<NeonLightGroup>(GetComponentsInChildren<NeonLightGroup>());

            Color randomColor = Random.ColorHSV(0, 1, 1, 1, 0, 1, 1, 1);

            foreach (NeonLightGroup neonLightSegment in neonLightGroups)
            {
                bool willDisappear = Random.Range(0f, 1f) < sceneConfig.NeonSignDisappearingChance;

                neonLightSegment.Initialize(randomColor, willDisappear);
            }
        }
    }
}
