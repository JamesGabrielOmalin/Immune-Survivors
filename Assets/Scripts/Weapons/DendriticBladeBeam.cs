using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DendriticBladeBeam : Projectile, IBodyColliderListener
{
    [SerializeField] private VisualEffect vfx;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackRange;
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
            float MaxHP = enemy.attributes.GetAttribute("Max HP").Value;
            float HP = enemy.attributes.GetAttribute("HP").Value;

            float ratio = Mathf.SmoothStep(0.25f, 0.75f, (HP / MaxHP));
            float missingHPBonusDMG = Mathf.Lerp(2f, 0.25f, ratio);

            // Reduce damage based on hit count, up to 50% reduction
            float damage = attackDamage * missingHPBonusDMG * (1f - Mathf.Min(0.1f * hitCount, 0.5f));

            float armor = enemy.attributes.GetAttribute("Armor").Value;

            DamageCalculator.ApplyDamage(damage * ratio, critRate, critDMG, armor, enemy);

            if (enemy.IsDead)
            {
                AudioManager.instance.Play("PlayerPickUp", transform.position);
                AntigenManager.instance.AddAntigen(enemy.Type);
            }

            hitCount++;
            var evt = vfx.CreateVFXEventAttribute();

            vfx.SetVector3("Hit Position", enemy.transform.position);
            vfx.SendEvent("OnHit", evt);
        }
    }

    //private IEnumerator Stop()
    //{
    //    WaitForSeconds wait = new(0.075f);

    //    yield return wait;
    //    vfx.Stop();
    //    yield return wait;
    //    this.gameObject.SetActive(false);
    //}

    public void OnBodyColliderExit(Collider other)
    {

    }
}
