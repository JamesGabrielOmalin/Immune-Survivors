using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DendriticJudgementCut : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    //[SerializeField] private SpriteRenderer sprite;
    [SerializeField] private VisualEffect vfx;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackSize;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;

    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(Slash());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Slash()
    {
        WaitForSeconds wait = new(0.5f);
        vfx.Play();

        var hits = Physics.OverlapSphere(transform.position, attackSize * 5f, layerMask.value);
        //float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                float armor = enemy.attributes.GetAttribute("Armor").Value;
                DamageCalculator.ApplyDamage(attackDamage, critRate, critDMG, armor, enemy);
                //enemy.TakeDamage(damage);
            }
        }

        yield return wait;
        this.gameObject.SetActive(false);
        yield break;
    }
}
