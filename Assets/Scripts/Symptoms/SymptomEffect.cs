using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Symptoms", menuName = "Symptoms/Symptom Effect")]

[System.Serializable]
public class SymptomEffect: ScriptableObject
{
    public enum SymptomEffectType
    {
        None = 0,
        Knockback = 1,
        DOT = 2,
        MoveSpeedBuff = 3,
    }

    public enum SymptomActivationType
    {
        Single,
        Loop,
    }

    public enum KnockbackDirection
    {
        Left = 0,
        Right = 1,
        Away = 2,
    }

    //public string Name;

    [field: SerializeField] public SymptomEffectType EffectType;
    //[field: SerializeField] float SymptomRadius;
    [field: SerializeField] public float ActivationDelay;
    [field: SerializeField] public SymptomActivationType ActivationType;

    // attributes shown based on custom editor Scripts/Editor/ SymptomEditor -> switch cases
    [Header ("Effect Attributes")]
    //knockback
    [field: SerializeField] float KnockbackIntensity;
    [field: SerializeField] KnockbackDirection Direction;


    [Header("Effect Attributes")]
    // dot
    [field: SerializeField] float DotDamage;
    [field: SerializeField] float DotDuration;
    [field: SerializeField] float DotTickRate;

    [Header("Effect Attributes")]
    // move speed
    [field: SerializeField] float MoveSpeedBuffAmount;
    [field: SerializeField] AttributeModifierType ModifierType;
    [field: SerializeField] float MoveSpeedBuffDuration;



    public void ActivateSymptom()
    {
       Debug.Log("Activated Symptom Effect: " + this.name);
        GameObject player = GameManager.instance.Player;

        switch (EffectType) 
        {
            case SymptomEffectType.Knockback:
                foreach(GameObject enemy in EnemyManager.instance.activeEnemies)
                {
                    //Debug.Log(collider.name);
                    Vector3 dir = Vector3.right;

                    if (enemy.TryGetComponent<Rigidbody>(out Rigidbody rb))
                    {

                        switch(Direction)
                        {
                            case KnockbackDirection.Left:
                                dir = Vector3.left;
                                break;

                            case KnockbackDirection.Right:
                                dir = Vector3.right;

                                break;

                            case KnockbackDirection.Away:
                                dir = enemy.transform.position - player.transform.position;
                                break;

                            
                        }
                        rb.AddForce(dir * KnockbackIntensity, ForceMode.Impulse);

                    }

                }

                break;

            case SymptomEffectType.DOT:
                foreach (GameObject enemy in EnemyManager.instance.activeEnemies)
                {
                    if (enemy.TryGetComponent<Enemy>(out Enemy enemyComp))
                    {
                        enemyComp.ApplyDoT(DotDamage, DotDuration, DotTickRate);
                    }
                }
                break;

            case SymptomEffectType.MoveSpeedBuff:
                AttributeModifier mod = new AttributeModifier(MoveSpeedBuffAmount, ModifierType);

                GameManager.instance.Player.GetComponent<Player>().ApplyMoveSpeedBuff(mod, MoveSpeedBuffDuration);
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
