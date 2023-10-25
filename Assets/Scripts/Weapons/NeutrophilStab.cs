using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class NeutrophilStab : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [HideInInspector] public Enemy target;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackCount;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float DoT;
    [HideInInspector] public float Type_1_DMG_Bonus;
    [HideInInspector] public float Type_2_DMG_Bonus;
    [HideInInspector] public float Type_3_DMG_Bonus;


    private readonly static WaitForSeconds wait = new WaitForSeconds(0.25f);
    //private readonly static WaitForSeconds delay = new WaitForSeconds(1f);

    protected void OnEnable()
    {
        StartCoroutine(Stab());
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Stab()
    {
        yield return null;

        vfx.SetInt("Count", (int)attackCount);
        vfx.Play();

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

        float damage = attackDamage * DMGBonus;
        float armor = target.Armor.Value;

        DamageCalculator.ApplyDamage(damage, critRate, critDMG, armor, target);
        target.ApplyDoT(DoT * DMGBonus, 3f, 4f + attackCount);

        yield return wait;

        vfx.Stop();

        this.gameObject.SetActive(false);
    }
}
