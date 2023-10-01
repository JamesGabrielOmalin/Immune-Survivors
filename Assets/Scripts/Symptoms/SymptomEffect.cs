using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Symptoms", menuName = "Symptoms/Symptom Effect")]

[System.Serializable]
public class SymptomEffect: ScriptableObject
{
   public enum TargetUnit
    {
        Player,
        Recruit,
        Enemy
    }

    public enum SymptomEffectType
    {
        None = 0,
        Knockback = 1,
        DoT = 2,
        MoveSpeedModifier = 3,
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
    [field: SerializeField] public TargetUnit AffectedUnit;

    [field: SerializeField] public SymptomEffectType EffectType;
    //[field: SerializeField] float SymptomRadius;
    [field: SerializeField] public SymptomActivationType ActivationType;

    [field: SerializeField] public float ActivationDelay;

    // attributes shown based on custom editor Scripts/Editor/ SymptomEditor -> switch cases
    [Header ("Effect Attributes")]
    //knockback
    [field: SerializeField] float KnockbackIntensity;
    [field: SerializeField] KnockbackDirection Direction;
    [field: SerializeField] int KnockbackCount;
    [field: SerializeField] float KnockbackInterval;




    [Header("Effect Attributes")]
    // dot
    [field: SerializeField] float DotDamage;
    [field: SerializeField] float DotDuration;
    [field: SerializeField] float DotTickRate;

    [Header("Effect Attributes")]
    // move speed
    [field: SerializeField] float MoveSpeedModifierAmount;
    [field: SerializeField] AttributeModifierType ModifierType;
    [field: SerializeField] bool IsInfiniteDuration = false;
    [field: SerializeField] float MoveSpeedModifierDuration;



    public void ActivateSymptom()
    {
       Debug.Log("Activated Symptom Effect: " + this.name);
        GameObject player = GameManager.instance.Player;

        switch (EffectType) 
        {
            case SymptomEffectType.Knockback:

                Vector3 dir = Vector3.right;
                switch (Direction)
                {
                    case KnockbackDirection.Left:
                        dir = Vector3.left;
                        break;

                    case KnockbackDirection.Right:
                        dir = Vector3.right;

                        break;
                }

                if (AffectedUnit == TargetUnit.Player)
                {
                    if(GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().TryGetComponent<PlayerUnit>(out PlayerUnit pu))
                    {
                        SymptomManager.instance.StartCoroutine(KnockbackCoroutine(pu,dir));
                        //pu.ApplyKnockback(dir * KnockbackIntensity, ForceMode.Impulse);
                    }
                }
                else if (AffectedUnit == TargetUnit.Enemy)
                {
                    foreach (GameObject enemy in EnemyManager.instance.activeEnemies)
                    {
                        if (enemy.TryGetComponent<Enemy>(out Enemy eu))
                        {
                            SymptomManager.instance.StartCoroutine(KnockbackCoroutine(eu, dir));

                            //eu.ApplyKnockback(dir * KnockbackIntensity, ForceMode.Impulse);
                        }
                    }
                }

                break;

            case SymptomEffectType.DoT:
                if (AffectedUnit == TargetUnit.Player)
                {
                    if (GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().TryGetComponent<PlayerUnit>(out PlayerUnit pu))
                    {
                        pu.ApplyDoT(DotDamage, DotDuration, DotTickRate);
                    }
                }
                else if (AffectedUnit == TargetUnit.Enemy)
                {
                    foreach (GameObject enemy in EnemyManager.instance.activeEnemies)
                    {
                        if (enemy.TryGetComponent<Enemy>(out Enemy enemyComp))
                        {
                            enemyComp.ApplyDoT(DotDamage, DotDuration, DotTickRate);
                        }
                    }
                }
                break;

            case SymptomEffectType.MoveSpeedModifier:


                if (AffectedUnit == TargetUnit.Player)
                {
                    AttributeModifier mod = new AttributeModifier(MoveSpeedModifierAmount, ModifierType);

                    GameManager.instance.Player.GetComponent<Player>().ApplyMoveSpeedModifier(mod, MoveSpeedModifierDuration, IsInfiniteDuration);
                }
                else if (AffectedUnit == TargetUnit.Enemy)
                {
                    if (IsInfiniteDuration)
                    {
                        foreach (GameObject enemy in EnemyManager.instance.allEnemies)
                        {
                            if (enemy.TryGetComponent<Enemy>(out Enemy enemyComp))
                            {
                                AttributeModifier mod = new AttributeModifier(MoveSpeedModifierAmount, ModifierType);

                                enemyComp.ApplyMoveSpeedModifier(mod, MoveSpeedModifierDuration, IsInfiniteDuration);
                            }
                        }
                    }
                    else
                    {
                        foreach (GameObject enemy in EnemyManager.instance.activeEnemies)
                        {
                            if (enemy.TryGetComponent<Enemy>(out Enemy enemyComp))
                            {
                                AttributeModifier mod = new AttributeModifier(MoveSpeedModifierAmount, ModifierType);

                                enemyComp.ApplyMoveSpeedModifier(mod, MoveSpeedModifierDuration, IsInfiniteDuration);
                            }
                        }
                    }
                   
                }
                
                break;


            default:
                break;
        }
    }

    private IEnumerator KnockbackCoroutine(Unit unit, Vector3 dir)
    {
         for (int i = 0; i < KnockbackCount; i++)
         {
            yield return new WaitForSeconds(KnockbackInterval);

            unit.ApplyKnockback(dir * KnockbackIntensity, ForceMode.Impulse);
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
