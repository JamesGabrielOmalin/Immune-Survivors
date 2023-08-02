using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SymptomEffect;

[CreateAssetMenu(fileName = "Symptoms", menuName = "Symptoms/Symptom Instance")]

[System.Serializable]
public class Symptom : ScriptableObject
{
    public string Name;
    //[field: SerializeField] public float ActivationDelay;
    //[field: SerializeField] public SymptomActivationType ActivationType;

    public List<SymptomEffect> symptomEffects = new();
    
}
