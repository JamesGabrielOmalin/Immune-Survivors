using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MacrophagePullType
{
    Line,
    Cone,
    Circle
}

public class MacrophagePull : MonoBehaviour
{
    [HideInInspector] public float abilityLevel;
    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float attackSize;
    [HideInInspector] public int attackCount;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float knockbackPower;
    [HideInInspector] public float DoT;
    [HideInInspector] public float Type_1_DMG_Bonus;
    [HideInInspector] public float Type_2_DMG_Bonus;
    [HideInInspector] public float Type_3_DMG_Bonus;

    [SerializeField] private MacrophagePullType type;
    [SerializeField] private LayerMask layer;

    private readonly WaitForSeconds wait = new(0.5f);

    private void OnEnable()
    {
        // Capture player position
        StartCoroutine(Pull());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Pull()
    {
        yield return null;

        if (attackDamage <= float.Epsilon)
        {
            yield return wait;
            this.gameObject.SetActive(false);
            yield break;
        }

        //targetPos = GameManager.instance.Player.transform.position;

        //float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);
        Collider[] hits = { };
        switch (type)
        {
            case MacrophagePullType.Line:
                hits = Physics.OverlapCapsule(transform.position, transform.position + (transform.forward * attackRange), 0.25f * (attackSize), layer.value);
                break;
            case MacrophagePullType.Cone:
                hits = Physics.OverlapSphere(transform.position, 2f * attackSize, layer.value);
                break;
            case MacrophagePullType.Circle:
                hits = Physics.OverlapSphere(transform.position, attackRange, layer.value);
                break;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out Enemy enemy))
            {
                float DMGBonus = Type_1_DMG_Bonus;

                switch (enemy.Type)
                {
                    case AntigenType.Type_1:
                        DMGBonus = Type_1_DMG_Bonus;
                        break;
                    case AntigenType.Type_2:
                        DMGBonus = Type_2_DMG_Bonus;
                        break;
                    case AntigenType.Type_3:
                        DMGBonus = Type_3_DMG_Bonus;
                        break;
                }

                float damage = attackDamage * DMGBonus;
                float armor = enemy.Armor.Value;
                DamageCalculator.ApplyDamage(damage, critRate, critDMG, armor, enemy);
                enemy.ApplyDoT(DoT * DMGBonus, 4f, 2f + attackCount);

                Vector3 dir = (enemy.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(dir * -knockbackPower, ForceMode.VelocityChange);
            }
        }

        yield return wait;
        this.gameObject.SetActive(false);
        yield break;
    }
}
