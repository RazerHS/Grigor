using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class PauseMenuWidget : UIWidget
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Slider masterVolume;
        [SerializeField] private Slider mouseSensitivity;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;

        public event Action BackButtonPressedEvent;
        public event Action<float> MasterVolumeChangedEvent;
        public event Action<float> MouseSensitivityChangedEvent;
        public event Action<int> ResolutionChangedEvent;
        public event Action<int> QualityChangedEvent;

        private bool dropdownsInitialized;

        protected override void OnShow()
        {
            backButton.onClick.AddListener(OnBackButtonPressed);

            masterVolume.onValueChanged.AddListener(OnMasterVolumeValueChanged);
            mouseSensitivity.onValueChanged.AddListener(OnMouseSensitivityValueChanged);
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        protected override void OnHide()
        {
            backButton.onClick.RemoveAllListeners();

            masterVolume.onValueChanged.RemoveAllListeners();
            mouseSensitivity.onValueChanged.RemoveAllListeners();
        }

        private void OnMouseSensitivityValueChanged(float sensitivity)
        {
            MouseSensitivityChangedEvent?.Invoke(sensitivity);
        }

        private void OnMasterVolumeValueChanged(float volume)
        {
            MasterVolumeChangedEvent?.Invoke(volume);
        }

        private void OnBackButtonPressed()
        {
            BackButtonPressedEvent?.Invoke();
        }

        private void OnResolutionChanged(int index)
        {
            ResolutionChangedEvent?.Invoke(index);
        }

        private void OnQualityChanged(int index)
        {
            QualityChangedEvent?.Invoke(index);
        }

        private void SetResolutionDropdownValues(List<TMP_Dropdown.OptionData> options)
        {
            resolutionDropdown.ClearOptions();

            resolutionDropdown.AddOptions(options);
        }

        private void SetQualityDropdownValues(List<TMP_Dropdown.OptionData> options)
        {
            qualityDropdown.ClearOptions();

            qualityDropdown.AddOptions(options);
        }

        public void InitializeDropdownOptions(List<TMP_Dropdown.OptionData> resolutionOptions, List<TMP_Dropdown.OptionData> qualityOptions)
        {
            if (dropdownsInitialized)
            {
                return;
            }

            SetResolutionDropdownValues(resolutionOptions);
            SetQualityDropdownValues(qualityOptions);

            dropdownsInitialized = true;
        }

        public void ForceSetResolutionValueInDropdown(int index)
        {
            resolutionDropdown.value = index;
        }

        public void ForceSetQualityValueInDropdown(int index)
        {
            qualityDropdown.value = index;
        }
    }
}
