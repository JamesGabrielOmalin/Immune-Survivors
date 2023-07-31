using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymptomManager : MonoBehaviour
{
    public GameObject player;
    public static SymptomManager instance;
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
    // Start is called before the first frame update
    void Start()
    {
        ActivateSymptoms();
    }

    private void ActivateSymptoms()
    {
        foreach (Symptom smp in SymptomList) 
        {
            foreach(SymptomEffect se in smp.symptomEffects)
            {
                StartCoroutine(se.SymptomCoroutine());
            }
        }
    }
}
