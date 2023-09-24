using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager instance;

    [Header("Post Processing")]
    [SerializeField] private VolumeProfile mainProfile;
    [SerializeField] private VolumeProfile playerVignetteprofile;

    private Vignette playerHitVignette;

    [Header("Player Vignette")]
    [SerializeField] float lerpDuration;
    [SerializeField] float targetIntensity;




    private Volume volume;
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
    private void OnEnable()
    {
        //GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().OnTakeDamage += TakeDamageVFX;

    }

    private void OnDisable()
    {
        //GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().OnTakeDamage -= TakeDamageVFX;

    }

    // Start is called before the first frame update
    void Start()
    {

        if (playerVignetteprofile.TryGet<Vignette>(out Vignette vignette))
        {
            playerHitVignette = vignette;
        }
        else
        {
            Debug.Log("Player hit vignette not found");
        }
        GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().OnTakeDamage += TakeDamageVFX;

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDamageVFX()
    {
        StartCoroutine(DamageVignetteCoroutine());
    }
    IEnumerator DamageVignetteCoroutine()
    {
        float time = 0;
        float startValue = playerHitVignette.intensity.value;
        while (time < lerpDuration)
        {
            playerHitVignette.intensity.value = Mathf.Lerp(startValue, targetIntensity, time / lerpDuration);
            time += Time.deltaTime;
            yield return null;
        }
        playerHitVignette.intensity.value = targetIntensity;

        startValue = playerHitVignette.intensity.value;

        time = 0;

        while (time < lerpDuration)
        {
            playerHitVignette.intensity.value = Mathf.Lerp(startValue, 0, time / lerpDuration);
            time += Time.deltaTime;
            yield return null;
        }

        playerHitVignette.intensity.value = 0;




    }
}
