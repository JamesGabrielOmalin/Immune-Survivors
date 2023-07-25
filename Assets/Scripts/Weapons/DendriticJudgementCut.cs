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
    [HideInInspector] public int attackCount;
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
        WaitForSeconds wait = new(0.5f / attackCount);
        vfx.Play();

        var hits = Physics.OverlapSphere(transform.position, attackSize * 5f, layerMask.value);
        float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);

        for (int i = 0; i < attackCount; i++)
        {
            //sprite.gameObject.SetActive(false);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                }
            }

            yield return wait;
            //sprite.gameObject.SetActive(true);
        }
        yield return null;
        this.gameObject.SetActive(false);
        yield break;
    }
}
