using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;

[Serializable]
public class Parameters
{
    public string name;
    public float distortionIntensity;
    public float particleSize;
    public Vector3 particleSpeed;
    public int spawnRate;
}
public class HeatDistortionController : MonoBehaviour
{
    public static HeatDistortionController instance;
    [SerializeField] private VisualEffect heatDistortVFX;

    [SerializeField] private List<Parameters> parameterList;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeVFXIntensity(int level)
    {
        StartCoroutine(HeatDistortionCoroutine(level));
    }

    private IEnumerator HeatDistortionCoroutine(int level)
    {
        PostProcessingManager.instance.HeatUpPostProcess(level);

        heatDistortVFX.SetInt("Spawn Rate", 0);

        yield return new WaitForSeconds(3.0f);
        Debug.Log("Changed Parameters to: " + parameterList[level].name);
        heatDistortVFX.SetFloat("Distortion Intensity", parameterList[level].distortionIntensity);
        heatDistortVFX.SetFloat("Particle Size", parameterList[level].particleSize);
        heatDistortVFX.SetVector3("Particle Speed", parameterList[level].particleSpeed);
        heatDistortVFX.SetInt("Spawn Rate", parameterList[level].spawnRate);

    }
}
