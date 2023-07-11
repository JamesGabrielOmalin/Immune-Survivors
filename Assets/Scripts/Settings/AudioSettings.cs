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

    [System.NonSerialized]
    public System.Action OnVolumeChanged;
    [System.NonSerialized]
    public System.Action OnMusicVolumeChanged;
    [System.NonSerialized]
    public System.Action OnSFXVolumeChanged;
}

public class AudioSettings : MonoBehaviour
{
    public static AudioSettingsData settings { get; private set; } = new();

    [SerializeField] private List<Sprite> volumeIconSprites;

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_InputField volumeInputField;
    [SerializeField] private Image volumeIcon;

    [Header("Music")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_InputField musicVolumeInputField;
    [SerializeField] private Image musicVolumeIcon;

    [Header("SFX")]
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_InputField sfxVolumeInputField;
    [SerializeField] private Image sfxVolumeIcon;

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
        settings.OnVolumeChanged?.Invoke();

        volumeInputField.text = settings.volume.ToString();

        switch (settings.volume)
        {
            case 0:
                volumeIcon.sprite = volumeIconSprites[0];
                break;
            case > 0 and <= 50:
                volumeIcon.sprite = volumeIconSprites[1];
                break;
            default:
                volumeIcon.sprite = volumeIconSprites[2];
                break;
        }
    }

    public void UpdateVolumeSlider(string value)
    {
        settings.volume = int.Parse(value);
        settings.OnVolumeChanged?.Invoke();

        volumeSlider.value = settings.volume;

        switch (settings.volume)
        {
            case 0:
                volumeIcon.sprite = volumeIconSprites[0];
                break;
            case > 0 and <= 50:
                volumeIcon.sprite = volumeIconSprites[1];
                break;
            default:
                volumeIcon.sprite = volumeIconSprites[2];
                break;
        }
    }

    public void UpdateMusicVolumeInputField(float value)
    {
        settings.musicVolume = (int)value;
        settings.OnMusicVolumeChanged?.Invoke();

        musicVolumeInputField.text = settings.musicVolume.ToString();

        switch (settings.musicVolume)
        {
            case 0:
                musicVolumeIcon.sprite = volumeIconSprites[0];
                break;
            case > 0 and <= 50:
                musicVolumeIcon.sprite = volumeIconSprites[1];
                break;
            default:
                musicVolumeIcon.sprite = volumeIconSprites[2];
                break;
        }
    }

    public void UpdateMusicVolumeSlider(string value)
    {
        settings.musicVolume = int.Parse(value);
        settings.OnMusicVolumeChanged?.Invoke();

        musicVolumeSlider.value = settings.musicVolume;

        switch (settings.musicVolume)
        {
            case 0:
                musicVolumeIcon.sprite = volumeIconSprites[0];
                break;
            case > 0 and <= 50:
                musicVolumeIcon.sprite = volumeIconSprites[1];
                break;
            default:
                musicVolumeIcon.sprite = volumeIconSprites[2];
                break;
        }
    }

    public void UpdateSFXVolumeInputField(float value)
    {
        settings.sfxVolume = (int)value;
        settings.OnSFXVolumeChanged?.Invoke();

        sfxVolumeInputField.text = settings.sfxVolume.ToString();

        switch (settings.sfxVolume)
        {
            case 0:
                sfxVolumeIcon.sprite = volumeIconSprites[0];
                break;
            case > 0 and <= 50:
                sfxVolumeIcon.sprite = volumeIconSprites[1];
                break;
            default:
                sfxVolumeIcon.sprite = volumeIconSprites[2];
                break;
        }
    }

    public void UpdateSFXVolumeSlider(string value)
    {
        settings.sfxVolume = int.Parse(value);
        settings.OnSFXVolumeChanged?.Invoke();

        sfxVolumeSlider.value = settings.sfxVolume;

        switch (settings.sfxVolume)
        {
            case 0:
                sfxVolumeIcon.sprite = volumeIconSprites[0];
                break;
            case > 0 and <= 50:
                sfxVolumeIcon.sprite = volumeIconSprites[1];
                break;
            default:
                sfxVolumeIcon.sprite = volumeIconSprites[2];
                break;
        }
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

    private void OnApplicationQuit()
    {
        Save();
    }
}
