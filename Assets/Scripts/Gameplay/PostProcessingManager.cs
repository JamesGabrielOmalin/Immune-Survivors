using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager instance;

    [Header("Post Processing")]
    [SerializeField] private VolumeProfile mainProfile;

    [Header("Vignette")]

    [SerializeField] Color defaultColor;
    [SerializeField] float defaultSmoothness;
    [SerializeField] float defaultIntensity;

    [SerializeField] Color targetColor;
    [SerializeField] float targetSmoothness;
    [SerializeField] float targetIntensity;

    [SerializeField] float lerpDuration;
    private Vignette mainVignette;


    [Header("Color Adjustment")]
    private ColorAdjustments colorAdjustment;
    [SerializeField] Color feverColor;

    [SerializeField] Color defaultColorAdjustment;
    [SerializeField] List<Color> targetColorList;
    [SerializeField] float transitionDuration;

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

        GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().OnTakeDamage += TakeDamagePostProcess;

        if (mainProfile.TryGet<ColorAdjustments>(out ColorAdjustments ca))
        {
            colorAdjustment = ca;

            colorAdjustment.colorFilter.value = Color.white;

        }
        else
        {
            Debug.Log("Color Adjustment not found");
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDamagePostProcess()
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

    public void HeatUpPostProcess(int level)
    {
        StartCoroutine(HeatUpColorAdjustmentCoroutine(level));
    }

    IEnumerator HeatUpColorAdjustmentCoroutine(int level)
    {
        float time = 0;
        Color startColor = defaultColorAdjustment;

        while (time < transitionDuration)
        {
            colorAdjustment.colorFilter.value = Color.Lerp(startColor, feverColor, time / transitionDuration);

            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(10f);
        time = 0;
        colorAdjustment.colorFilter.value = feverColor;
        startColor = feverColor;

        while (time < transitionDuration)
        {
            colorAdjustment.colorFilter.value = Color.Lerp(startColor, targetColorList[level], time / transitionDuration);

            time += Time.deltaTime;
            yield return null;
        }

        colorAdjustment.colorFilter.value = targetColorList[level];
        defaultColorAdjustment = targetColorList[level];
    }

}
