using UnityEngine;

public class DamageCalculator
{
    public static float CalcDamage(in float attackDamage, in float critRate, in float critDMG)
    {
        bool isCrit = Random.value <= critRate;
        return attackDamage * (isCrit ? critDMG : 1f);
    }
}
