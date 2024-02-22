using System.Collections.Generic;
using Grigor.Data;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    public class HouseVisuals : MonoBehaviour
    {
        [SerializeField] private List<NeonLightGroup> neonLightGroups;

        public void Awake()
        {
            neonLightGroups = new List<NeonLightGroup>(GetComponentsInChildren<NeonLightGroup>());

            Color randomColor = Random.ColorHSV();

            foreach (NeonLightGroup neonLightSegment in neonLightGroups)
            {
                bool willDisappear = Random.Range(0f, 1f) < SceneConfig.Instance.NeonSignDisappearingChance;

                neonLightSegment.Initialize(randomColor, willDisappear);
            }
        }
    }
}
