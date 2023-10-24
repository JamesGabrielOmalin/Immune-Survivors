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

        float armor = target.Armor.Value;

        DamageCalculator.ApplyDamage(attackDamage, critRate, critDMG, armor, target);
        target.ApplyDoT(DoT, 3f, 4f + attackCount);

        yield return wait;

        vfx.Stop();

        this.gameObject.SetActive(false);
    }
}
