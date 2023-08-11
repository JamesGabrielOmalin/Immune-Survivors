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

    protected void OnEnable()
    {
        StartCoroutine(Stab());
    }

    private IEnumerator Stab()
    {
        yield return null;

        WaitForSeconds wait = new(0.25f / attackCount);
        vfx.SetInt("Count", (int)attackCount);
        vfx.Play();

        float armor = target.attributes.GetAttribute("Armor").Value;

        DamageCalculator.ApplyDamage(attackDamage, critRate, critDMG, armor, target);
        target.ApplyDoT(DoT, 3f, 4f + attackCount);

        yield return wait;

        vfx.Stop();

        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
