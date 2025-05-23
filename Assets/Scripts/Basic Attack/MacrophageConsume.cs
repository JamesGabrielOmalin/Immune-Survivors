using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacrophageConsume : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float attackSize;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float knockbackPower;
    [HideInInspector] public float dot;
    [HideInInspector] public float duration;
    [HideInInspector] public int tickRate;
    [HideInInspector] public float Type_1_DMG_Bonus;
    [HideInInspector] public float Type_2_DMG_Bonus;
    [HideInInspector] public float Type_3_DMG_Bonus;

    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(Consume());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Consume()
    {
        yield return new WaitForSeconds(1f);

        var hits = Physics.OverlapSphere(transform.position, attackSize, layerMask);

        //float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);


        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
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

                float armor = enemy.attributes.GetAttribute(Attribute.ARMOR).Value;
                //enemy.TakeDamage(damage);
                DamageCalculator.ApplyDamage(attackDamage * DMGBonus, critRate, critDMG, armor, enemy);
                enemy.ApplyDoT(dot, duration, tickRate);

                // Pull effect
                if (enemy.TryGetComponent<ImpactReceiver>(out ImpactReceiver impactReceiver))
                {
                    Vector3 dir = Vector3.Normalize(enemy.transform.position - transform.position);
                    impactReceiver.AddImpact(dir, -knockbackPower); 
                }
            }
        }

        Destroy(this.gameObject);
    }
}
