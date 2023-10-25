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
    [HideInInspector] public float Type_1_DMG_Bonus;
    [HideInInspector] public float Type_2_DMG_Bonus;
    [HideInInspector] public float Type_3_DMG_Bonus;

    private int hitCount = 0;

    protected override void OnEnable()
    {
        lifeSpan = attackRange / projectileSpeed;
        vfx.SetFloat("Lifetime", lifeSpan);
        base.OnEnable();
    }

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            float MaxHP = enemy.MaxHP.Value;
            float HP = enemy.HP.Value;

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

            float ratio = Mathf.SmoothStep(0.25f, 0.75f, (HP / MaxHP));
            float missingHPBonusDMG = Mathf.Lerp(2f, 0.25f, ratio);

            // Reduce damage based on hit count, up to 50% reduction
            float damage = attackDamage * DMGBonus  * missingHPBonusDMG * (1f - Mathf.Min(0.1f * hitCount, 0.5f));

            float armor = enemy.Armor.Value;

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
