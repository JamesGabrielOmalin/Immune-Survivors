using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

public class GraphicsSettingsData
{
    public int graphicsQuality = 2;
    public int resolution = 0;
    public int displayMode = 0;
    public int fps = 1;
    public int vsync = 0;
}

public class GraphicsSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown graphicsQualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    [SerializeField] private TMP_Dropdown fpsDropdown;
    [SerializeField] private TMP_Dropdown vsyncDropdown;

    private static GraphicsSettingsData settings = new();

    private void Awake()
    {
        Load();

        SetGraphicsQuality(settings.graphicsQuality);
        SetResolution(settings.resolution);
        SetDisplayMode(settings.displayMode);
        SetFPS(settings.fps);
        SetVSync(settings.vsync);
    }

    private void Start()
    {
        graphicsQualityDropdown.value = settings.graphicsQuality;

        resolutionDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> resolutions = new();

        foreach (var res in Screen.resolutions)
        {
            resolutions.Add(new($"{res.width} x {res.height}"));
        }

        resolutions.Reverse();
        resolutionDropdown.AddOptions(resolutions);
        resolutionDropdown.value = settings.resolution;

        displayModeDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> displayModes = new();

        displayModes.Add(new("Fullscreen"));
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.LinuxPlayer:
                displayModes.Add(new("Windowed"));
                break;
        }

        displayModeDropdown.AddOptions(displayModes);
        displayModeDropdown.value = settings.displayMode;

        fpsDropdown.value = settings.fps;

        vsyncDropdown.value = settings.vsync;
    }

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);

        settings.graphicsQuality = index;
    }

    public void SetResolution(int index)
    {
        var res = Screen.resolutions;
        Screen.SetResolution(res[index].width, res[index].height, Screen.fullScreenMode);

        settings.resolution = index;
    }

    public void SetDisplayMode(int index)
    {
        if (settings.displayMode == 0)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;

        settings.displayMode = index;
    }

    public void SetFPS(int index)
    {
        switch (index)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 120;
                break;
        }

        settings.fps = index;
    }

    public void SetVSync(int index)
    {
        switch (index)
        {
            case 0:
                QualitySettings.vSyncCount = 0;
                break;
            case 1:
                QualitySettings.vSyncCount = 1;
                break;
        }

        settings.vsync = index;
    }

    private void OnDestroy()
    {
        Save();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(settings);
        File.WriteAllText(Application.persistentDataPath + "/GraphicsSettings.json", json);
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/GraphicsSettings.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/GraphicsSettings.json");
            settings = JsonUtility.FromJson<GraphicsSettingsData>(json);
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
