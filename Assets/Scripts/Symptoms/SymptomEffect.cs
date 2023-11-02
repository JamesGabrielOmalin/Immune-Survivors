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
        Top = 2,
        Bottom = 3,
        Away = 4,
        Random = 5,

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
    public KnockbackDirection KnockDirection;
    public int KnockbackCount;
    [field: SerializeField] float KnockbackInterval;
     Vector3 dir = Vector3.right;

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

                dir = GetKnockbackDirection(KnockDirection);

                if (AffectedUnit == TargetUnit.Player)
                {
                    if(GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().TryGetComponent<PlayerUnit>(out PlayerUnit pu))
                    {

                        SymptomManager.instance.StartCoroutine(KnockbackCoroutine(pu,dir, KnockbackIntensity));
                        //pu.ApplyKnockback(dir * KnockbackIntensity, ForceMode.Impulse);
                    }
                }
                else if (AffectedUnit == TargetUnit.Enemy)
                {

                    //if (GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().TryGetComponent<PlayerUnit>(out PlayerUnit pu))
                    //{

                    //    SymptomManager.instance.StartCoroutine(KnockbackCoroutine(pu, dir, KnockbackIntensity/2));
                    //    //pu.ApplyKnockback(dir * KnockbackIntensity, ForceMode.Impulse);
                    //}

                    foreach (GameObject enemy in EnemyManager.instance.activeEnemies)
                    {
                        if (enemy.TryGetComponent<Enemy>(out Enemy eu))
                        {
                            if (KnockDirection == KnockbackDirection.Away)
                            {
                                dir = enemy.transform.position - player.transform.position;
                            }
                            SymptomManager.instance.StartCoroutine(KnockbackCoroutine(eu, dir, KnockbackIntensity));

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
                            if (KnockDirection == KnockbackDirection.Away)
                            {
                                dir = enemy.transform.position - player.transform.position;
                            }
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

    private IEnumerator KnockbackCoroutine(Unit unit, Vector3 dir, float intensity)
    {

        for (int i = 0; i < KnockbackCount; i++)
        {
            float duration = 1;
            float delay = KnockbackInterval - 1.5f;
            CoughPingController.instance.ActivatePing(true, KnockDirection,delay, duration);

            yield return new WaitForSeconds(KnockbackInterval);

            SymptomManager.instance.coughVFX.transform.position = GameManager.instance.Player.transform.position;
            SymptomManager.instance.coughVFX.Play();

            //CoughPingController.instance.ActivatePing(false, KnockDirection, 0);

            unit.ApplyKnockback(dir * intensity, ForceMode.Impulse);
         }
        SymptomManager.instance.ActivateCoughCameraCue(false,2);
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

    private Vector3 GetKnockbackDirection(KnockbackDirection kdir)
    {
        Vector3 direction = Vector3.zero;
        int index = Random.Range(1, 4);

        Debug.Log(index);
        switch (kdir)
        {
            case KnockbackDirection.Left:
                direction = Vector3.right;
                //CoughPingController.instance.StartCoroutine(CoughPingController.instance.PingCoroutine("RIGHT", ActivationDelay-1));
                break;

            case KnockbackDirection.Right:
                direction = Vector3.left;
                //CoughPingController.instance.StartCoroutine(CoughPingController.instance.PingCoroutine("LEFT", ActivationDelay-1));
                break;

            case KnockbackDirection.Bottom:
                direction = Vector3.forward;
                //CoughPingController.instance.StartCoroutine(CoughPingController.instance.PingCoroutine("BOTTOM", ActivationDelay-1));
                break;

            case KnockbackDirection.Top:
                direction = -Vector3.forward;
                //CoughPingController.instance.StartCoroutine(CoughPingController.instance.PingCoroutine("TOP", ActivationDelay-1));
                break;


            case KnockbackDirection.Random:
                direction = GetKnockbackDirection((KnockbackDirection)Random.Range(0, 3));
                break;
        }

        return direction;
    }

}
