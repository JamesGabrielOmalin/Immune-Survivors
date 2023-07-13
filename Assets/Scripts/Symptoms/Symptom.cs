using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Symptom Effect", menuName = "Symptom/Symptom Effect")]

[System.Serializable]
public class Symptom : ScriptableObject
{
    public enum SymptomEffectType
    {
        None = 0,
        Knockback = 1,
    }

    public enum SymptomActivationType
    {
        Single,
        Loop,
    }

    public string Name;    // Start is called before the first frame update

    [field: SerializeField] public SymptomEffectType EffectType;
    [field: SerializeField] float SymptomRadius;
    [field: SerializeField] public float ActivationDelay;
    [field: SerializeField] public SymptomActivationType ActivationType;

    // attributes shown based on custom editor Scripts/Editor/ SymptomEditor -> switch cases
    [Header ("Effect Attributes")]
    [field: SerializeField] float KnockbackIntensity;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateSymptom()
    {
        Debug.Log("Activate: " + this.Name);
        GameObject player = GameManager.instance.Player;

        switch (EffectType) 
        {
            case SymptomEffectType.Knockback:

                Collider[] colliderArray = Physics.OverlapSphere(player.transform.position, SymptomRadius, LayerMask.GetMask("Enemy"));
                foreach(Collider collider in colliderArray)
                {
                    //Debug.Log(collider.name);
                    Vector3 dir = collider.transform.position - player.transform.position;

                    if (collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
                    {
                        rb.AddForce(dir.normalized * KnockbackIntensity, ForceMode.Impulse);
                    }
                     
                }

                break;
            default:
                break;
        }
    }

    public IEnumerator SymptomCoroutine()
    {
        do
        {
            yield return new WaitForSeconds(ActivationDelay);

            ActivateSymptom();

        }
        while (ActivationType == SymptomActivationType.Loop);
    }

}
