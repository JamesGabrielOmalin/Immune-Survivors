using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [HideInInspector] public Sound sound;
    // Start is called before the first frame update
    private void Start()
    {
        if (GameManager.instance)
        {
            GameManager.instance.OnGamePaused += audioSource.Pause;
            GameManager.instance.OnGameResumed += audioSource.UnPause;
            GameManager.instance.OnGameResumed += delegate
            {
                audioSource.volume = (sound.volume * (AudioSettings.settings.sfxVolume / 100f)) * (AudioSettings.settings.volume / 100f);
            };
        }
    }
}
