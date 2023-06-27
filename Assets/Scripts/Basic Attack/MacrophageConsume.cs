using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacrophageConsume : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float knockbackPower;
    [HideInInspector] public float dot;
    [HideInInspector] public float duration;
    [HideInInspector] public float tickRate;

    // Start is called before the first frame update
    private void Start()
    {
        transform.localScale = Vector3.one * attackRange;

        StartCoroutine(Consume());
    }

    private IEnumerator Consume()
    {
        yield return new WaitForSeconds(1f);

        var hits = Physics.OverlapSphere(transform.position, attackRange, layerMask);

        float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
                enemy.ApplyDoT(dot, duration, tickRate);

                // Pull effect
                if (enemy.TryGetComponent<ImpactReceiver>(out ImpactReceiver impactReceiver))
                {
                    Vector3 dir = Vector3.Normalize(enemy.transform.position - transform.position);
                    impactReceiver.AddImpact(dir, -knockbackPower); 
                }
            }
        }

        Destroy(this.gameObject);
    }
}
