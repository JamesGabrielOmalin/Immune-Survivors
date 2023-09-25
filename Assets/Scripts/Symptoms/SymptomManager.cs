using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class SymptomAttribute
{
    public Symptom symptom;
    public int activationTimestamp = 0;
}
public class SymptomManager : MonoBehaviour
{
    public GameObject player;
    public static SymptomManager instance;
    public bool isActive = false;
    [SerializeField] private List<SymptomAttribute> SymptomList;
    [SerializeField] private int SymptomLevel = 1;
    [SerializeField] private int symptomTimer = 0;

    public System.Action OnSymptomThresholdNotReached;
    public System.Action OnSymptomThresholdReached;

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

    private void OnDestroy()
    {
        instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        //EnemyManager.instance.OnSymptomThresholdReached += ActivateSymptoms;
        //EnemyManager.instance.OnSymptomThresholdNotReached += DeactivateSyptoms;

        StartCoroutine(SymptomTimerCoroutine());

    }

    private void ActivateSymptoms()
    {
        HeatDistortionController.instance.ChangeVFXIntensity(SymptomLevel);
        foreach(SymptomEffect se in SymptomList[SymptomLevel-1].symptom.symptomEffects)
        {
            StartCoroutine(se.SymptomCoroutine());
        }
    }

    private void DeactivateSyptoms()
    {
        foreach (SymptomEffect se in SymptomList[SymptomLevel - 1].symptom.symptomEffects)
        {
            StopCoroutine(se.SymptomCoroutine());
        }
    }

    IEnumerator SymptomTimerCoroutine()
    {
        do
        {
            yield return new WaitForSeconds(1);

            symptomTimer++;

            if (symptomTimer == SymptomList[SymptomLevel - 1].activationTimestamp)
            {
                Debug.Log("Activate symptoms");
                ActivateSymptoms();
                SymptomLevel++;

                if (SymptomLevel >= SymptomList.Count)
                {
                    isActive = false;
                }
            }

        } while (isActive);
    }

   
}
