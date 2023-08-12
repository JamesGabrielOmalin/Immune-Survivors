using UnityEngine;

public class DamageCalculator
{
    private const float DMG_REDUCTION_FACTOR = 10f;
    public static float CalcDamage(in float attackDamage, in float critRate, in float critDMG)
    {
        bool isCrit = Random.value <= critRate;
        return attackDamage * (isCrit ? critDMG : 1f);
    }

    public static void ApplyDamage(in float attackDamage, in float armor, IDamageInterface target)
    {
        if (attackDamage <= float.Epsilon)
        {
            return;
        }

        float reduction = DMG_REDUCTION_FACTOR / (DMG_REDUCTION_FACTOR + armor);
        float damage = attackDamage * reduction;

        if (damage <= float.Epsilon)
        {
            return;
        }

        target.TakeDamage(damage);
    }

    public static void ApplyDamage(in float attackDamage, in float critRate, in float critDMG, in float armor, IDamageInterface target)
    {
        if (attackDamage <= float.Epsilon)
        {
            return;
        }

        bool isCrit = Random.value < critRate;
        float reduction = DMG_REDUCTION_FACTOR / (DMG_REDUCTION_FACTOR + armor);
        float damage = attackDamage * (isCrit ? critDMG : reduction);

        if (damage <= float.Epsilon)
        {
            return;
        }

        target.TakeDamage(damage);

        Vector3 location = (target as MonoBehaviour).transform.position;
        location += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0, 0.5f), Random.Range(-0.5f, 0.5f));

        DamageNumberManager.instance.SpawnDamageNumber(location, damage, isCrit);
    }
}
