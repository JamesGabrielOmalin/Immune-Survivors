using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DendriticBladeBeam : Projectile, IBodyColliderListener
{
    [SerializeField] private VisualEffect vfx;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackRange;
    [HideInInspector] public int attackCount;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;

    private int hitCount = 0;

    protected override void OnEnable()
    {
        lifeSpan = attackRange / projectileSpeed;
        vfx.SetFloat("Lifetime", lifeSpan);
        base.OnEnable();
    }

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            // Reduce damage based on hit count, up to 50% reduction
            float damage = DamageCalculator.CalcDamage(attackDamage * (1f - Mathf.Min(0.1f * hitCount, 0.5f)), critRate, critDMG);

            enemy.TakeDamage(damage);
            // Apply knockback
            //if (enemy.TryGetComponent<ImpactReceiver>(out ImpactReceiver impactReceiver))
            //    impactReceiver.AddImpact(transform.forward, knockbackPower);
            hitCount++;
            var evt = vfx.CreateVFXEventAttribute();

            vfx.SetVector3("Hit Position", enemy.transform.position);
            vfx.SendEvent("OnHit", evt);

            Debug.Log("Blade Beam Hit!");

            if (hitCount >= attackCount)
            {
                StopAllCoroutines();
                StartCoroutine(Stop());
            }
        }
    }

    private IEnumerator Stop()
    {
        WaitForSeconds wait = new(0.075f);

        yield return wait;
        vfx.Stop();
        yield return wait;
        this.gameObject.SetActive(false);
    }

    public void OnBodyColliderExit(Collider other)
    {

    }
}
