using UnityEngine;

public class DamageCalculator
{
    private const float DMG_REDUCTION_FACTOR = 10f;
    public static float CalcDamage(in float attackDamage, in float critRate, in float critDMG)
    {
        bool isCrit = Random.value <= critRate;
        return attackDamage * (isCrit ? critDMG : 1f);
    }

    public static void ApplyDamage(in float attackDamage, in float critRate, in float critDMG, in float armor, IDamageInterface target)
    {
        bool isCrit = Random.value <= critRate;
        float reduction = DMG_REDUCTION_FACTOR / DMG_REDUCTION_FACTOR + armor;
        float damage = attackDamage * (isCrit ? critDMG : reduction);

        target.TakeDamage(damage);
    }
}
