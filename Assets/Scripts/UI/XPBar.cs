using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPBar : MonoBehaviour
{
    [SerializeField] private Slider bar;

    private void Start()
    {
        RecruitManager.instance.OnThreshholdUpdate += UpdateBar;
    }

    private void OnDestroy()
    {
        //RecruitManager.instance.OnThreshholdUpdate -= UpdateBar;
    }

    public void UpdateBar()
    {
        bar.value = (float)RecruitManager.instance.killCount / (float)RecruitManager.instance.GetCurrentKillThreshold();

        Debug.Log(bar.value);
    }
}
