using CardboardCore.DI;
using Grigor.Gameplay.Audio;
using Grigor.Gameplay.Lighting;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Time
{
    public class Level : CardboardCoreBehaviour, ITimeEffect
    {
        [SerializeField, ColoredBoxGroup("Level", false, true)] private LevelName levelName;
        [SerializeField, ColoredBoxGroup("Level")] private Transform spawnPoint;
        [SerializeField, ColoredBoxGroup("Level")] private GameObject roomView;

        [SerializeField, ColoredBoxGroup("Audio", false, true)] protected bool playAudio;
        [SerializeField, ColoredBoxGroup("Audio"), ShowIf(nameof(playAudio))] protected bool playAmbienceAudio;
        [SerializeField, ColoredBoxGroup("Audio"), ShowIf("@playAudio"), ValueDropdown("@GameConfig.Instance.GetAudioEvents()")] protected string dayMusic;
        [SerializeField, ColoredBoxGroup("Audio"), ShowIf("@playAudio"), ValueDropdown("@GameConfig.Instance.GetAudioEvents()")] protected string nightMusic;
        [SerializeField, ColoredBoxGroup("Audio"), ShowIf("@playAudio && playAmbienceAudio"), ValueDropdown("@GameConfig.Instance.GetAudioEvents()")] protected string dayAmbience;
        [SerializeField, ColoredBoxGroup("Audio"), ShowIf("@playAudio && playAmbienceAudio"), ValueDropdown("@GameConfig.Instance.GetAudioEvents()")] protected string nightAmbience;

        [Inject] private LevelRegistry levelRegistry;
        [Inject] private AudioController audioController;
        [Inject] private TimeEffectRegistry timeEffectRegistry;
        [Inject] private TimeManager timeManager;

        public Transform SpawnPoint => spawnPoint;
        public LevelName LevelName => levelName;

        public LevelLighting LevelLighting { get; private set; }

        protected override void OnInjected()
        {
            LevelLighting = GetComponentInChildren<LevelLighting>(true);

            levelRegistry.Register(levelName, this);

            RegisterTimeEffect();
        }

        protected override void OnReleased()
        {
            levelRegistry.Unregister(levelName);
        }

        public void EnableLevel()
        {
            roomView.SetActive(true);
        }

        public void DisableLevel()
        {
            roomView.SetActive(false);
        }

        private void PlayAudio(string music, string ambience)
        {
            if (!playAudio)
            {
                return;
            }

            audioController.PlaySoundLooping(music);

            if (!playAmbienceAudio)
            {
                return;
            }

            audioController.PlaySoundLooping(ambience);
        }

        public void OnChangedToDay()
        {
            // audioController.StopSound(nightMusic, true, 2f);
            // audioController.StopSound(nightAmbience, true, 2f);
            //
            // audioController.PlaySoundLooping(dayMusic);
            // audioController.PlaySoundLooping(dayAmbience);
        }

        public void OnChangedToNight()
        {
            // audioController.StopSound(dayMusic, true, 2f);
            // audioController.StopSound(dayAmbience, true, 2f);
            //
            // audioController.PlaySoundLooping(nightMusic);
            // audioController.PlaySoundLooping(nightAmbience);
        }

        public void RegisterTimeEffect()
        {
            timeEffectRegistry.Register(this);
        }
    }
}
