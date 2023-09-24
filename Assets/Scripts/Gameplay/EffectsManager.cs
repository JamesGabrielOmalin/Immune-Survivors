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

    private Vignette mainVignette;

    [Header("Player Vignette")]
    [SerializeField] Color defaultColor;
    [SerializeField] float defaultSmoothness;
    [SerializeField] float defaultIntensity;


    [SerializeField] Color targetColor;
    [SerializeField] float targetSmoothness;
    [SerializeField] float targetIntensity;

    [SerializeField] float lerpDuration;





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

        if (mainProfile.TryGet<Vignette>(out Vignette vignette))
        {
            mainVignette = vignette;

            mainVignette.color.value = defaultColor;
            mainVignette.smoothness.value = defaultSmoothness;
            mainVignette.intensity.value = defaultIntensity;
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
        Color startColor = mainVignette.color.value;
        float startSmoothness = defaultSmoothness;
        float startIntensity = defaultIntensity;



        while (time < lerpDuration)
        {
            mainVignette.color.value = Color.Lerp(startColor, targetColor, time / lerpDuration);
            mainVignette.smoothness.value = Mathf.Lerp(startSmoothness, targetSmoothness, time / lerpDuration);
            mainVignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, time / lerpDuration);
            time += Time.deltaTime;
            yield return null;
        }
        mainVignette.color.value = targetColor;
        mainVignette.smoothness.value = targetSmoothness;
        mainVignette.intensity.value = targetIntensity;

        startColor = mainVignette.color.value;
        startSmoothness = mainVignette.smoothness.value;
        startIntensity = mainVignette.intensity.value;


        time = 0;

        while (time < lerpDuration)
        {
            mainVignette.color.value = Color.Lerp(startColor, defaultColor, time / lerpDuration);
            mainVignette.smoothness.value = Mathf.Lerp(startSmoothness, defaultSmoothness, time / lerpDuration);
            mainVignette.intensity.value = Mathf.Lerp(startIntensity, defaultIntensity, time / lerpDuration);
            time += Time.deltaTime;
            yield return null;
        }

        mainVignette.color.value = defaultColor;
        mainVignette.smoothness.value = defaultSmoothness;
        mainVignette.intensity.value = defaultIntensity;
    }
}
