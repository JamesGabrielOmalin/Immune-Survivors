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

    protected void OnEnable()
    {
        StartCoroutine(Stab());
    }

    private IEnumerator Stab()
    {
        WaitForSeconds wait = new(0.25f / attackCount);
        vfx.SetInt("Count", (int)attackCount);
        vfx.Play();

        float armor = target.attributes.GetAttribute("Armor").Value;

        for (int i = 0; i < attackCount; i++)
        {
            //if (Vector3.Distance(target.transform.position, GameManager.instance.Player.transform.position) >= attackRange)
            //    break;

            //float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);
            //target.TakeDamage(damage);

            DamageCalculator.ApplyDamage(attackDamage, critRate, critDMG, armor, target);

            yield return wait;
        }

        vfx.Stop();

        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
