using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Data;
using Grigor.Gameplay.Audio;
using TMPro;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Grigor.Gameplay.Settings
{
    [Injectable]
    public class SettingsManager : CardboardCoreBehaviour
    {
        [Inject] private AudioController audioController;
        [Inject] private CharacterRegistry characterRegistry;

        private SceneConfig sceneConfig;
        private int currentResolutionIndex;
        private int currentQualityIndex;

        public int CurrentResolutionIndex => currentResolutionIndex;
        public int CurrentQualityIndex => currentQualityIndex;

        protected override void OnInjected()
        {
            sceneConfig = SceneConfig.Instance;
        }

        protected override void OnReleased()
        {

        }

        public void OnMouseSensitivityChanged(float sensitivity)
        {
            characterRegistry.Player.Look.SetLookSensitivity(sensitivity);
        }

        public void OnMasterVolumeChanged(float volume)
        {
            audioController.SetMasterVolume(volume);
        }

        public void OnResolutionToggled(int index)
        {
            currentResolutionIndex = index;

            Vector2 resolution = sceneConfig.ResolutionOptions[currentResolutionIndex];

            // HDRenderPipelineAsset hdAsset = GraphicsSettings.defaultRenderPipeline as HDRenderPipelineAsset;
            //
            // var dynamicResSettings = hdAsset.currentPlatformRenderPipelineSettings.dynamicResolutionSettings;
            //
            // dynamicResSettings.enabled = true;
            //
            // dynamicResSettings.forceResolution = true;
            // dynamicResSettings.forcedPercentage = 10f;
        }

        // NOTE: the settings from the previous session remain because the volume profile asset has changed, so the quality has to be reset
        public void OnQualityChanged(int index)
        {
            currentQualityIndex = index;

            QualitySettings.SetQualityLevel(index);

            Log.Write($"Changed quality to {QualitySettings.names[index]}!");

            QualityOptions quality = sceneConfig.QualityOptions[currentQualityIndex];

            sceneConfig.OnQualityChanged(quality);
        }

        public List<TMP_Dropdown.OptionData> GetResolutionDropdownOptions()
        {
            List<Vector2> resolutions = sceneConfig.ResolutionOptions;

            List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();

            foreach (Vector2 resolution in resolutions)
            {
                optionData.Add(new TMP_Dropdown.OptionData($"{resolution.x} x {resolution.y}"));
            }

            return optionData;
        }

        public List<TMP_Dropdown.OptionData> GetQualityDropdownOptions()
        {
            List<QualityOptions> qualityOptions = sceneConfig.QualityOptions;

            List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();

            foreach (QualityOptions quality in qualityOptions)
            {
                optionData.Add(new TMP_Dropdown.OptionData(quality.ToString()));
            }

            return optionData;
        }
    }
}
