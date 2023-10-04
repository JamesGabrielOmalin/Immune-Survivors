using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPBar : MonoBehaviour
{
    [SerializeField] private Slider bar;
    private Coroutine lerpCoroutine;

    [SerializeField] private float lerpSpeed = 0.1f;

    private void Start()
    {
        RecruitManager.instance.OnThreshholdUpdate += UpdateBar;
    }

    private void OnDestroy()
    {
        //RecruitManager.instance.OnThreshholdUpdate -= UpdateBar;
        OnDisable();
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        if (RecruitManager.instance)
            RecruitManager.instance.OnThreshholdUpdate += UpdateBar;
    }

    private void OnDisable()
    {
        if(lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        if (RecruitManager.instance)
            RecruitManager.instance.OnThreshholdUpdate -= UpdateBar;
    }

    public void UpdateBar()
    {
        if(lerpCoroutine != null) 
        {
            StopCoroutine(lerpCoroutine);
        }

        if (this.enabled)
            lerpCoroutine = StartCoroutine(Lerp());
    }

    private IEnumerator Lerp()
    {
        float t = 0;
        float start = bar.value;
        float end = (float)RecruitManager.instance.killCount / (float)RecruitManager.instance.GetCurrentKillThreshold();

        while(t < lerpSpeed)
        {
            bar.value = Mathf.Lerp(start, end, t/lerpSpeed);

            t += Time.deltaTime;

            yield return null;
        }

        lerpCoroutine = null;
    }
}
