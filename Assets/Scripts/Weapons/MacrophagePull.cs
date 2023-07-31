using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MacrophagePullType
{
    Line,
    Cone,
    Circle
}

public class MacrophagePull : MonoBehaviour
{
    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float attackSize;
    [HideInInspector] public int attackCount;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float knockbackPower;

    [SerializeField] private MacrophagePullType type;
    [SerializeField] private LayerMask layer;
    private Vector3 targetPos;

    private void OnEnable()
    {
        // Capture player position
        StartCoroutine(Pull());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Pull()
    {
        yield return null;
        targetPos = GameManager.instance.Player.transform.position;

        float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);
        int hitCount = 0;
        Collider[] hits = { };
        switch (type)
        {
            case MacrophagePullType.Line:
                hits = Physics.OverlapCapsule(transform.position, transform.position + (transform.forward * attackRange), 0.5f * (attackSize + attackRange), layer.value);
                break;
            case MacrophagePullType.Cone:
                hits = Physics.OverlapSphere(transform.position, 2f * attackSize, layer.value);
                break;
            case MacrophagePullType.Circle:
                hits = Physics.OverlapSphere(transform.position, (attackSize + attackRange), layer.value);
                break;
        }

        for (int i = 0; i < hits.Length && hitCount < attackCount; i++)
        {
            if (hits[i].TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
                enemy.ApplyDoT(damage, 4f, attackCount);
                if (enemy.TryGetComponent<ImpactReceiver>(out ImpactReceiver impact))
                {
                    Vector3 dir = (enemy.transform.position - targetPos).normalized;
                    impact.AddImpact(dir, -knockbackPower);
                }
                hitCount++;
            }
        }

        yield return new WaitForSeconds(0.15f);
        this.gameObject.SetActive(false);
        yield break;
    }
}
