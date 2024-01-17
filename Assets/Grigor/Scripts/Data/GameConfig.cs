using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Grigor.Gameplay.Cats;
using Grigor.Gameplay.Interacting;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Grigor.Data
{
    [GlobalConfig("Assets/Resources/Config/"), CreateAssetMenu(fileName = "Config", menuName = "Grigor/Game Config")]
    public class GameConfig : GlobalConfig<GameConfig>
    {
        [SerializeField, ColoredBoxGroup("Gameplay", true, true)] private int correctCluesBeforeLock = 3;
        [SerializeField, ColoredBoxGroup("Gameplay")] private float timePassTweenDuration = 1f;

        [SerializeField, ColoredBoxGroup("Cats", true, true), AssetsOnly]  private Interactable catPrefab;
        [SerializeField, ColoredBoxGroup("Cats")] private Catnip catnipPrefab;
        [SerializeField, ColoredBoxGroup("Cats"), Range(0f, 50f)] private float catnipRadius = 10f;
        [SerializeField, ColoredBoxGroup("Cats"), Range(0f, 1f)] private float chanceForCatToSpawnNearCatnip = 0.5f;
        [SerializeField, ColoredBoxGroup("Cats"), Range(0f, 50f)] private float catnipUpwardsForceWhenThrown = 5f;
        [SerializeField, ColoredBoxGroup("Cats"), Range(0f, 5f)] private float catnipReactionDelay = 1f;

        [SerializeField, ColoredBoxGroup("Story Graph", true, 0.2f, 0.5f, 0.5f)] private Color startNodeColor;
        [SerializeField, ColoredBoxGroup("Story Graph")] private Color defaultNodeColor;
        [SerializeField, ColoredBoxGroup("Story Graph")] private Color endNodeColor;
        [SerializeField, ColoredBoxGroup("Story Graph")] private Color noSpeakerColor;

        [SerializeField, ColoredBoxGroup("Audio", false, true), AssetSelector, LabelText("FMOD Event Cache")] private EventCache fmodEventCache;

        public int CorrectCluesBeforeLock => correctCluesBeforeLock;
        public float TimePassTweenDuration => timePassTweenDuration;

        public float CatnipRadius => catnipRadius;
        public Interactable CatPrefab => catPrefab;
        public float ChanceForCatToSpawnNearCatnip => chanceForCatToSpawnNearCatnip;
        public Catnip CatnipPrefab => catnipPrefab;
        public float CatnipUpwardsForceWhenThrown => catnipUpwardsForceWhenThrown;
        public float CatnipReactionDelay => catnipReactionDelay;

        public Color StartNodeColor => startNodeColor;
        public Color DefaultNodeColor => defaultNodeColor;
        public Color EndNodeColor => endNodeColor;
        public Color NoSpeakerColor => noSpeakerColor;

        public List<string> GetAudioEvents()
        {
            return fmodEventCache.EditorEvents.Select(e => e.Path.Replace("event:/", "")).ToList();
        }
    }
}
