using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Toggle lowPerformanceToggle;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Text versionText;
    
    void Start()
    {
        LoadSettings();
    }

    void LoadSettings()
    {
        GameSettings settings = GameManager.instance.currentSave.settings;
        
        if (musicToggle != null)
            musicToggle.isOn = settings.music;
        
        if (sfxToggle != null)
            sfxToggle.isOn = settings.sfx;
        
        if (lowPerformanceToggle != null)
            lowPerformanceToggle.isOn = settings.lowPerformanceMode;
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = settings.musicVolume;
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = settings.sfxVolume;
        
        if (versionText != null)
            versionText.text = "版本: 1.0";
    }

    public void OnMusicToggle(bool value)
    {
        GameManager.instance.currentSave.settings.music = value;
        GameManager.instance.SaveGame();
    }

    public void OnSfxToggle(bool value)
    {
        GameManager.instance.currentSave.settings.sfx = value;
        GameManager.instance.SaveGame();
    }

    public void OnLowPerformanceToggle(bool value)
    {
        GameManager.instance.currentSave.settings.lowPerformanceMode = value;
        GameManager.instance.SaveGame();
    }

    public void OnMusicVolumeChanged(float value)
    {
        GameManager.instance.currentSave.settings.musicVolume = value;
        GameManager.instance.SaveGame();
    }

    public void OnSfxVolumeChanged(float value)
    {
        GameManager.instance.currentSave.settings.sfxVolume = value;
        GameManager.instance.SaveGame();
    }

    public void OnClearSaveButton()
    {
        GameManager.instance.ClearSave();
        UIManager.instance.ShowMessage("存档已清除！");
        LoadSettings();
    }

    public void OnBackButton()
    {
        UIManager.instance.OnBackButton();
    }
}
