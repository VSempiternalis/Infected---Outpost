using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class Settings : MonoBehaviour {
    [SerializeField] private AudioMixer mainMixer;
    private Resolution[] resolutions;

    [Header("UI")]
    [Space(10)]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private void Start() {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;

        List<string> options = new List<string>();
        for(int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        LoadSettings();
    }

    public void RestoreDefaults() {
        print("Loading default settings");
        SetVolume(0); //-80 to 20

        SetQuality(2);
        SetResolution(resolutions.Length - 1);
        SetFullscreen(true);
    }

    private void LoadSettings() {
        //AUDIO
        //Volume
        float volume = PlayerPrefs.GetFloat("Settings_Volume, 60f");
        SetVolume(volume);
        
        //VIDEO
        //Quality
        int qualityIndex = PlayerPrefs.GetInt("Settings_Quality", 2);
        SetQuality(qualityIndex);
        //Fullscreen
        // SetFullscreen((PlayerPrefs.GetInt("Settings_IsFullscreen") == 1 ? true : false));
        //Resolution
        int resolutionIndex = PlayerPrefs.GetInt("Settings_ResolutionIndex");
        SetResolution(resolutionIndex);
    }

    public void SetVolume(float volume) {
        mainMixer.SetFloat("Volume", volume);

        //Saving
        PlayerPrefs.SetFloat("Settings_Volume", volume);
        
        //Update UI
        volumeSlider.value = volume;
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);

        //Saving
        PlayerPrefs.SetInt("Settings_Quality", qualityIndex);

        //Update UI
        graphicsDropdown.value = qualityIndex;
    }

    public void SetFullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
        fullscreenToggle.isOn = isFullscreen;
    }

    public void SetResolution(int resolutionIndex) {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        //Saving
        PlayerPrefs.SetInt("Settings_ResolutionIndex", resolutionIndex);

        //Update UI
        resolutionDropdown.value = resolutionIndex;
    }
}