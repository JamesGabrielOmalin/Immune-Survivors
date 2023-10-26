using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    // Start is called before the first frame update
    private void Start()
    {
        if (GameManager.instance)
        {
            GameManager.instance.OnGamePaused += audioSource.Pause;
            GameManager.instance.OnGameResumed += audioSource.UnPause;
        }
    }
}
