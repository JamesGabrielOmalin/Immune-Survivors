using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.VFX;  

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
    [SerializeField] private float zoomedAmount;
    [SerializeField] public VisualEffect coughVFX;


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
        defaultFOV = vCam.m_Lens.FieldOfView;
        StartCoroutine(SymptomTimerCoroutine());
    }

    private void ActivateSymptoms()
    {
        OnActivateSymptom?.Invoke();

        if (symptomType == SymptomType.Fever)
        {
            HeatDistortionController.instance.ChangeVFXIntensity(SymptomLevel);
        }
        else if (symptomType == SymptomType.Cough)
        {
            SymptomManager.instance.ActivateCoughCameraCue(true, 0);
        }

        foreach (SymptomEffect se in SymptomList[SymptomLevel-1].symptom.symptomEffects)
        {
            StartCoroutine(se.SymptomCoroutine());

            if (symptomType == SymptomType.Cough)
            {
                switch (se.KnockDirection)
                {
                    case SymptomEffect.KnockbackDirection.Left:
                        coughVFX.transform.forward = Vector3.right;
                        break;
                    case SymptomEffect.KnockbackDirection.Right:
                        coughVFX.transform.forward = Vector3.left;
                        break;
                    case SymptomEffect.KnockbackDirection.Top:
                        coughVFX.transform.forward = Vector3.back;
                        break;
                    case SymptomEffect.KnockbackDirection.Bottom:
                        coughVFX.transform.forward = Vector3.forward;
                        break;
                    case SymptomEffect.KnockbackDirection.Away:
                        break;
                    case SymptomEffect.KnockbackDirection.Random:
                        var rand = UnityEngine.Random.insideUnitCircle;
                        coughVFX.transform.forward = new(rand.x, 0f, rand.y);
                        break;
                }
            }
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

            float activationTime = SymptomList[SymptomLevel - 1].activationTimestamp;

            if (symptomTimer == activationTime)
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


    public void ActivateCoughCameraCue( bool isZoomOut, float delay)
    {
        StartCoroutine(CoughCameraZoomCoroutine( isZoomOut, delay));
    }

    private IEnumerator CoughCameraZoomCoroutine( bool isZoomOut, float delay)
    {
 
        float time = 0;

        float startingFOV;


        Debug.Log("FOV: " + vCam.m_Lens.FieldOfView);

        yield return new WaitForSeconds(delay);

        if (isZoomOut)
        {
            startingFOV = vCam.m_Lens.FieldOfView;
            float targetFOV = defaultFOV + zoomedAmount;
            // Starting Zoom Out
            while (time < transitionDuration)
            {
                vCam.m_Lens.FieldOfView = Mathf.Lerp(startingFOV, targetFOV, time);

                time += Time.deltaTime;
                yield return null;
            }

            vCam.m_Lens.FieldOfView = targetFOV;
        }
        else
        {
            startingFOV = vCam.m_Lens.FieldOfView;
            float targetFOV = defaultFOV - zoomedAmount;

            // Zoom In
            while (time < transitionDuration)
            {
                vCam.m_Lens.FieldOfView = Mathf.Lerp(startingFOV, targetFOV, time );

                time += Time.deltaTime;
                yield return null;
            }
            vCam.m_Lens.FieldOfView = targetFOV;
        }
        //// Starting Zoom Out
        //while (time < transitionDuration)
        //{
        //    vCam.m_Lens.FieldOfView = Mathf.Lerp(startingFOV, defaultFOV+zoomedOutFOV, time / (delay*0.25f));

        //    time += Time.deltaTime;
        //    yield return null;
        //}

        //vCam.m_Lens.FieldOfView = defaultFOV + zoomedOutFOV;
        //startingFOV = defaultFOV + zoomedOutFOV;

        //for (int i = 0; i < cycle; i++)
        //{
        //    time = 0;

        //    // Zoom In
        //    while (time < 1)
        //    {
        //        vCam.m_Lens.FieldOfView = Mathf.Lerp(startingFOV, defaultFOV - zoomedInFOV, time / 0.1f);

        //        time += Time.deltaTime;
        //        //yield return null;
        //    }
        //    vCam.m_Lens.FieldOfView = defaultFOV - zoomedInFOV;
        //    startingFOV = defaultFOV - zoomedInFOV;

        //    time = 0;
        //    while (time < 1)
        //    {
        //        vCam.m_Lens.FieldOfView = Mathf.Lerp(startingFOV, defaultFOV, time / 0.5f);

        //        time += Time.deltaTime;
        //        yield return null;
        //    }
        //    vCam.m_Lens.FieldOfView = defaultFOV;
        //    startingFOV = defaultFOV;
        //}
    }


}
