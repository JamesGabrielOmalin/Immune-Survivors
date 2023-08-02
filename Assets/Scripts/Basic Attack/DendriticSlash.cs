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
        float armor = target.attributes.GetAttribute("Armor").Value;

        for (int i = 0; i < attackCount; i++)
        {
            DamageCalculator.ApplyDamage(attackDamage, critRate, critDMG, armor, target);
            //target.TakeDamage(damage);

            yield return wait;
        }

        yield return null;
        this.gameObject.SetActive(false);
        yield break;
    }
}
