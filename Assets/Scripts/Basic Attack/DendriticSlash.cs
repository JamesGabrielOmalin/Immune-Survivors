using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DendriticSlash : MonoBehaviour
{
    [HideInInspector] public Enemy target;
    //[SerializeField] private LayerMask layerMask;
    //[SerializeField] private SpriteRenderer sprite;
    [SerializeField] private VisualEffect vfx;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public int attackCount;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float Type_1_DMG_Bonus;
    [HideInInspector] public float Type_2_DMG_Bonus;
    [HideInInspector] public float Type_3_DMG_Bonus;


    private void OnEnable()
    {
        StartCoroutine(Slash());
    }

    private void OnDisable()
    {

    }

    private IEnumerator Slash()
    {
        yield return null;
        if (!target)
        {
            yield break;
        }

        WaitForSeconds wait = new(0.25f / attackCount);
        vfx.SetInt("Count", attackCount);
        vfx.Play();

        //float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);
        float MaxHP = target.MaxHP.Value;
        float HP = target.HP.Value;

        float armor = target.Armor.Value;

        float ratio = Mathf.SmoothStep(0.25f, 0.75f, (HP / MaxHP));

        float DMGBonus = Type_1_DMG_Bonus;

        switch (target.Type)
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

        // Quarter damage against full health, double damage
        float missingHPBonusDMG = Mathf.Lerp(2f, 0.25f, ratio);
        Debug.Log($"Bonus DMG: {missingHPBonusDMG}");

        float damage = attackDamage * DMGBonus * missingHPBonusDMG;

        for (int i = 0; i < attackCount; i++)
        {
            DamageCalculator.ApplyDamage(damage, critRate, critDMG, armor, target);

            // Gain bonus Antigen if enemy is killed
            if (target.IsDead)
            {
                AudioManager.instance.Play("PlayerPickUp", transform.position);
                AntigenManager.instance.AddAntigen(target.Type);
                break;
            }

            //target.TakeDamage(damage);

            yield return wait;
        }

        yield return null;
        this.gameObject.SetActive(false);
        yield break;
    }
}
