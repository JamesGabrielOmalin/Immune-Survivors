using Cinemachine;
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

public enum SymptomType
{
    Fever,
    Cough,
}

public class SymptomManager : MonoBehaviour
{
    public GameObject player;
    public static SymptomManager instance;
    public SymptomType symptomType = SymptomType.Fever;
    public bool isActive = false;
    [SerializeField] private List<SymptomAttribute> SymptomList;
    [SerializeField] private int SymptomLevel = 1;
    [SerializeField] private int symptomTimer = 0;

    [Header ("Cough")]
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float zoomedInFOV;
    [SerializeField] private float zoomedOutFOV;


    public System.Action OnActivateSymptom;

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
        StartCoroutine(SymptomTimerCoroutine());
    }

    private void ActivateSymptoms()
    {
        OnActivateSymptom?.Invoke();

        if (symptomType == SymptomType.Fever)
        {
            HeatDistortionController.instance.ChangeVFXIntensity(SymptomLevel);
        }

        foreach(SymptomEffect se in SymptomList[SymptomLevel-1].symptom.symptomEffects)
        {

            if (symptomType == SymptomType.Cough)
            {
                CoughPingController.instance.ActivatePing(se.KnockDirection, se.ActivationDelay);
                ActivateCoughVisualCue(se.ActivationDelay,se.KnockbackCount);

            }
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


    private void ActivateCoughVisualCue(float delay, float cycle)
    {
        StartCoroutine(CoughCameraEffectCoroutine(delay, cycle));
    }

    private IEnumerator CoughCameraEffectCoroutine(float delay, float cycle)
    {
        defaultFOV = vCam.m_Lens.FieldOfView;
        float startingFOV = defaultFOV;
        float time = 0;

        Debug.Log("FOV: " + vCam.m_Lens.FieldOfView);
        // Starting Zoom Out
        while (time < transitionDuration)
        {
            vCam.m_Lens.FieldOfView = Mathf.Lerp(startingFOV, defaultFOV+zoomedOutFOV, time / (delay*0.25f));

            time += Time.deltaTime;
            yield return null;
        }

        vCam.m_Lens.FieldOfView = defaultFOV + zoomedOutFOV;
        startingFOV = defaultFOV + zoomedOutFOV;

        for (int i = 0; i < cycle; i++)
        {
            time = 0;

            // Zoom In
            while (time < 1)
            {
                vCam.m_Lens.FieldOfView = Mathf.Lerp(startingFOV, defaultFOV - zoomedInFOV, time / 0.1f);

                time += Time.deltaTime;
                //yield return null;
            }
            vCam.m_Lens.FieldOfView = defaultFOV - zoomedInFOV;
            startingFOV = defaultFOV - zoomedInFOV;

            time = 0;
            while (time < 1)
            {
                vCam.m_Lens.FieldOfView = Mathf.Lerp(startingFOV, defaultFOV, time / 0.5f);

                time += Time.deltaTime;
                yield return null;
            }
            vCam.m_Lens.FieldOfView = defaultFOV;
            startingFOV = defaultFOV;
        }
    }

   
}
