using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioSource musicSource;

    [SerializeField] private GameObject audiosource;
    [SerializeField] private ObjectPool Audiopool;

    public static AudioManager instance;

    public float SFXMasterVolume;
    public bool SFXMasterMute = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        /*foreach (Sound s in sounds)
        {
            if (s.name == "Theme") { }
            else 
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
            
        }*/
    }

    // Start is called before the first frame update
    private void Start()
    {
        PlayMusic("Theme");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
        {
            musicSource.clip = s.clips[0];
            musicSource.volume = s.volume;
            musicSource.pitch = s.pitch;
            musicSource.loop = s.loop;
            musicSource.Play();
           
        }
    }

    public void AddAudioSource(GameObject gameobject)
    {
        AudioSource audioSource = gameobject.AddComponent<AudioSource>();

        if(audioSource == null)
        {
            audioSource = gameobject.AddComponent<AudioSource>();
        }

    }

    public void Play (string name, Vector3 loc)
    {
       Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null) 
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        int randonNumber = Random.Range(0, s.clips.Length);

        GameObject temp = Audiopool.RequestPoolable(loc);
        AudioSource audioSource = temp.GetComponent<AudioSource>();

        audioSource.clip = s.clips[randonNumber];
        audioSource.volume = (s.volume * (AudioSettings.settings.sfxVolume / 100)) * (AudioSettings.settings.volume / 100);
        audioSource.pitch = s.pitch;
        audioSource.loop = s.loop;
        audioSource.mute = SFXMasterMute;
        audioSource.Play();
        Debug.LogWarning("SFXPlaying");

        StartCoroutine(DeleteAudioObject(audioSource, s.clips[randonNumber]));
    }

    private IEnumerator DeleteAudioObject(AudioSource audioSource, AudioClip clip)
    {
        WaitForSeconds wait = new(clip.length);

        yield return wait;
        Debug.LogWarning("DeletedAudioObject");
        //audioSource.clip = null;
        Destroy(audioSource.gameObject);

        yield break;
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        SFXMasterMute= !SFXMasterMute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = musicSource.volume * volume;
    }

    public void SFXVolume(float volume)
    {
        SFXMasterVolume = volume;
    }
}
