using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class NeutrophilBullet : Projectile, IBodyColliderListener
{
    [SerializeField] private VisualEffect vfx;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float knockbackPower;

    private bool hit = false;

    public void OnBodyColliderEnter(Collider other)
    {
        if (hit)
            return;

        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            //float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);

            //enemy.TakeDamage(damage);
            float armor = enemy.attributes.GetAttribute("Armor").Value;
            DamageCalculator.ApplyDamage(attackDamage, critRate, critDMG, armor, enemy);

            // Apply knockback
            if (enemy.TryGetComponent(out ImpactReceiver impactReceiver))
                impactReceiver.AddImpact(transform.forward, knockbackPower);

            vfx.SetBool("Alive", false);
            vfx.SendEvent("Kill");
            StartCoroutine(Deactivate());

            hit = true;
        }
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(0.25f);
        this.gameObject.SetActive(false);
        hit = false;
        vfx.SetBool("Alive", true);
    }

    public void OnBodyColliderExit(Collider other)
    {

    }
}
