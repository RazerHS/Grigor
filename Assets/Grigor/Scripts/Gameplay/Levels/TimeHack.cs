using System;
using Grigor.Data;
using UnityEngine;

namespace Grigor.Gameplay.Time
{
    public class TimeHack : MonoBehaviour
    {
        private SceneConfig sceneConfig;

        private void Awake()
        {
            sceneConfig = SceneConfig.Instance;
        }

        private void Update()
        {

        }
    }
}
