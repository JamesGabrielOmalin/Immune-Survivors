using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SymptomManager : MonoBehaviour
{
    public GameObject player;
    public static SymptomManager instance;
    public bool isActive = false;
    [SerializeField] private List<Symptom> SymptomList;

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
        EnemyManager.instance.OnSymptomThresholdReached += ActivateSymptoms;
        EnemyManager.instance.OnSymptomThresholdNotReached += DeactivateSyptoms;

    }

    private void ActivateSymptoms()
    {
        if (isActive)
        {
            return;
        }

        isActive = true;

        foreach (Symptom smp in SymptomList) 
        {
            Debug.Log("Activated symptom coroutine");
            foreach(SymptomEffect se in smp.symptomEffects)
            {
                StartCoroutine(se.SymptomCoroutine());
            }
        }
    }

    private void DeactivateSyptoms()
    {
        if (!isActive)
        {
            return;
        }

        isActive = false;
        foreach (Symptom smp in SymptomList)
        {
            Debug.Log("Activated symptom coroutine");
            foreach (SymptomEffect se in smp.symptomEffects)
            {
                StopCoroutine(se.SymptomCoroutine());
            }
        }
    }
}
