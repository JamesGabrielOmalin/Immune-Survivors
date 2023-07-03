using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettingsData
{
    public int volume = 100;
    public int musicVolume = 100;
    public int sfxVolume = 100;
}

public class AudioSettings : MonoBehaviour
{
    private static AudioSettingsData settings = new();

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_InputField volumeInputField;

    [Header("Music")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_InputField musicVolumeInputField;

    [Header("SFX")]
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_InputField sfxVolumeInputField;

    private void Awake()
    {
        Load();
    }

    // Start is called before the first frame update
    private void Start()
    {
        volumeSlider.value = settings.volume;
        volumeInputField.text = settings.volume.ToString();

        musicVolumeSlider.value = settings.musicVolume;
        musicVolumeInputField.text = settings.musicVolume.ToString();

        sfxVolumeSlider.value = settings.sfxVolume;
        sfxVolumeInputField.text = settings.sfxVolume.ToString();
    }

    public void UpdateVolumeInputField(float value)
    {
        settings.volume = (int)value;
        volumeInputField.text = settings.volume.ToString(); 
    }

    public void UpdateVolumeSlider(string value)
    {
        settings.volume = int.Parse(value);
        volumeSlider.value = settings.volume;
    }

    public void UpdateMusicVolumeInputField(float value)
    {
        settings.volume = (int)value;
        musicVolumeInputField.text = settings.volume.ToString();
    }

    public void UpdateMusicVolumeSlider(string value)
    {
        settings.volume = int.Parse(value);
        musicVolumeSlider.value = settings.volume;
    }

    public void UpdateSFXVolumeInputField(float value)
    {
        settings.volume = (int)value;
        sfxVolumeInputField.text = settings.volume.ToString();
    }

    public void UpdateSFXVolumeSlider(string value)
    {
        settings.volume = int.Parse(value);
        sfxVolumeSlider.value = settings.volume;
    }

    private void OnDestroy()
    {
        Save();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(settings);
        File.WriteAllText(Application.persistentDataPath + "/AudioSettings.json", json);
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/AudioSettings.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/AudioSettings.json");
            settings = JsonUtility.FromJson<AudioSettingsData>(json);
        }
    }
}
