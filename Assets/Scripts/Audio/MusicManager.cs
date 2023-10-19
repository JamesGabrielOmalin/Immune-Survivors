using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioSource mainBGM;
    [SerializeField] private AudioSource lastMinuteBGM;
    [SerializeField] private AudioSource infectionBGM;

    private AudioSource activeBGM;
    private bool isTransitioning = false;

    [SerializeField] private float crossfadeDuration;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    private void Start()
    {
        if (AudioSettings.settings != null)
        {
            AudioSettings.settings.OnVolumeChanged += OnVolumeChanged;
            AudioSettings.settings.OnMusicVolumeChanged += OnVolumeChanged;
        }

        if (GameManager.instance)
            GameManager.instance.OnLastMinuteReached += TransitionToLastMinuteBGM;

        float volume = (AudioSettings.settings.volume / 100f) * (AudioSettings.settings.musicVolume / 100f);
        activeBGM = mainBGM;
        activeBGM.volume = volume;
    }

    private void OnDestroy()
    {
        if (AudioSettings.settings != null)
        {
            AudioSettings.settings.OnVolumeChanged -= OnVolumeChanged;
            AudioSettings.settings.OnMusicVolumeChanged -= OnVolumeChanged;
        }

        activeBGM.Stop();
        instance = null;
    }

    public void TransitionToMainBGM()
    {
        StartCoroutine(Crossfade(mainBGM));
    }

    public void TransitionToLastMinuteBGM()
    {
        StartCoroutine(Crossfade(lastMinuteBGM));
    }

    public void TransitionToInfectionBGM()
    {
        StartCoroutine(Crossfade(infectionBGM));
    }

    private IEnumerator Crossfade(AudioSource newBGM)
    {
        float t = 0f;

        newBGM.Play(); 
        isTransitioning = true;

        while (t < crossfadeDuration)
        {
            float ratio = t / crossfadeDuration;
            float volume = (AudioSettings.settings.volume / 100f) * (AudioSettings.settings.musicVolume / 100f);
            activeBGM.volume = ratio * volume;
            lastMinuteBGM.volume = (1f - ratio) * volume;

            t += Time.deltaTime;
            yield return null;
        }

        activeBGM.Stop();
        activeBGM = newBGM; 
        isTransitioning = false;
    }

    private void OnVolumeChanged()
    {
        if (isTransitioning)
            return;

        activeBGM.volume = (AudioSettings.settings.volume / 100f) * (AudioSettings.settings.musicVolume / 100f);
    }
}
