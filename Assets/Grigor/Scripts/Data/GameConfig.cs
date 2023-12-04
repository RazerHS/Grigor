using RazerCore.Utils.Attributes;
using Sirenix.Utilities;
using UnityEngine;

namespace Grigor.Data
{
    [GlobalConfig, CreateAssetMenu(fileName = "Config", menuName = "Grigor/GameConfig")]
    public class GameConfig : GlobalConfig<GameConfig>
    {
        [SerializeField, ColoredBoxGroup("Sky and Ambience Color", true, 0.2f, 0.5f, 0.5f)] private Gradient ambientColor;
        [SerializeField, ColoredBoxGroup("Sky and Ambience Color")] private Gradient directionalColor;
        [SerializeField, ColoredBoxGroup("Sky and Ambience Color")] private Gradient fogColor;

        [SerializeField, ColoredBoxGroup("Gameplay", true, true)] private int correctCluesBeforeLock = 3;

        [SerializeField, ColoredBoxGroup("Story Graph", true, 0.2f, 0.5f, 0.5f)] private Color startNodeColor;
        [SerializeField, ColoredBoxGroup("Story Graph")] private Color defaultNodeColor;
        [SerializeField, ColoredBoxGroup("Story Graph")] private Color endNodeColor;
        [SerializeField, ColoredBoxGroup("Story Graph")] private Color noSpeakerColor;

        public Gradient AmbientColor => ambientColor;
        public Gradient DirectionalColor => directionalColor;
        public Gradient FogColor => fogColor;

        public int CorrectCluesBeforeLock => correctCluesBeforeLock;

        public Color StartNodeColor => startNodeColor;
        public Color DefaultNodeColor => defaultNodeColor;
        public Color EndNodeColor => endNodeColor;
        public Color NoSpeakerColor => noSpeakerColor;
    }
}
