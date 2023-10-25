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
    [HideInInspector] public float Type_1_DMG_Bonus;
    [HideInInspector] public float Type_2_DMG_Bonus;
    [HideInInspector] public float Type_3_DMG_Bonus;

    private bool hit = false;

    public void OnBodyColliderEnter(Collider other)
    {
        if (hit)
            return;

        if (other.TryGetComponent(out Enemy enemy))
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

            //enemy.TakeDamage(damage);
            float armor = enemy.Armor.Value;
            DamageCalculator.ApplyDamage(damage, critRate, critDMG, armor, enemy);

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
