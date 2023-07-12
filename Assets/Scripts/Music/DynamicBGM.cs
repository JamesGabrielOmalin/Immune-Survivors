using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBGM : MonoBehaviour
{
    [SerializeField] private AudioSource bgm_A;
    [SerializeField] private AudioSource bgm_B;
    [SerializeField] private AudioLowPassFilter filter;

    // Start is called before the first frame update
    private void Start()
    {
        UpdateBGM();

        GameManager.instance.OnGamePaused += OnGamePaused;
        GameManager.instance.OnGameResumed += OnGameResumed;

        AudioSettings.settings.OnVolumeChanged += OnVolumeChanged;
        AudioSettings.settings.OnMusicVolumeChanged += OnVolumeChanged;

        EnemyManager.instance.OnInfectionRateChanged += UpdateBGM;
    }

    private void OnDestroy()
    {
        AudioSettings.settings.OnVolumeChanged -= OnVolumeChanged;
        AudioSettings.settings.OnMusicVolumeChanged -= OnVolumeChanged;
    }

    private void OnVolumeChanged()
    {
        UpdateBGM();
    }

    private void UpdateBGM()
    {
        float infectionRatio = (float)EnemyManager.instance.InfectionRate / EnemyManager.instance.MaxInfectionRate;
        float volume = (AudioSettings.settings.volume / 100f) * (AudioSettings.settings.musicVolume / 100f);

        float lerpA = Mathf.Lerp(volume, 0f, infectionRatio);
        float lerpB = Mathf.Lerp(0, volume, infectionRatio);

        bgm_A.volume = lerpA;
        bgm_B.volume = lerpB;
    }

    private void OnGamePaused()
    {
        filter.enabled = true;
    }

    private void OnGameResumed()
    {
        filter.enabled = false;
    }
}
