using RazerCore.Utils.Attributes;
using Sirenix.Utilities;
using UnityEngine;

namespace Grigor.Gameplay.Lighting
{
    [GlobalConfig, CreateAssetMenu(fileName = "Config", menuName = "Grigor/GameConfig")]
    public class GameConfig : GlobalConfig<GameConfig>
    {
        [SerializeField, ColoredBoxGroup("Sky and Ambience Color", true, 0.2f, 0.5f, 0.5f)] private Gradient ambientColor;
        [SerializeField, ColoredBoxGroup("Sky and Ambience Color")] private Gradient directionalColor;
        [SerializeField, ColoredBoxGroup("Sky and Ambience Color")] private Gradient fogColor;

        public Gradient AmbientColor => ambientColor;
        public Gradient DirectionalColor => directionalColor;
        public Gradient FogColor => fogColor;
    }
}
