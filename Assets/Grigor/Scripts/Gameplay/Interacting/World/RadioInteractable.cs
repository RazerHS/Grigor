using System.Collections.Generic;
using FMOD.Studio;
using Grigor.Gameplay.Interacting.Components;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class RadioInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Radio", false, true), ValueDropdown("@GameConfig.Instance.GetAudioEvents()")] protected List<string> songs;

        [ShowInInspector, ColoredBoxGroup("Debug"), ReadOnly] private bool muted;
        [ShowInInspector, ColoredBoxGroup("Debug"), ReadOnly] private int currentSongIndex = -1;

        private EventInstance currentSongInstance;

        protected override void OnInitialized()
        {
            PlayNewSong();

            CheckMute();
        }

        protected override void OnInteractEffect()
        {
            muted = !muted;

            CheckMute();

            EndInteract();
        }

        private void Update()
        {
            if (IsCurrentSongStillPlaying())
            {
                return;
            }

            PlayNewSong();
        }

        private bool IsCurrentSongStillPlaying()
        {
            currentSongInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

            return playbackState != PLAYBACK_STATE.STOPPING;
        }

        private void PlayNewSong()
        {
            currentSongIndex++;

            currentSongIndex %= songs.Count;

            currentSongInstance = AudioController.PlaySound3D(songs[currentSongIndex], transform);

            CheckMute();
        }

        private void CheckMute()
        {
            if (muted)
            {
                AudioController.MuteSound(songs[currentSongIndex], false);

                return;
            }

            AudioController.UnmuteSound(songs[currentSongIndex], false);
        }
    }
}
