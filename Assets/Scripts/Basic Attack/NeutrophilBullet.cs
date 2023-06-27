using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutrophilBullet : Projectile, IBodyColliderListener
{
    [HideInInspector] public float attackDamage;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float knockbackPower;

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);

            enemy.TakeDamage(damage);
            // Apply knockback
            if (enemy.TryGetComponent<ImpactReceiver>(out ImpactReceiver impactReceiver))
                impactReceiver.AddImpact(transform.forward, knockbackPower);

            this.gameObject.SetActive(false);
        }
    }

    public void OnBodyColliderExit(Collider other)
    {

    }
}
