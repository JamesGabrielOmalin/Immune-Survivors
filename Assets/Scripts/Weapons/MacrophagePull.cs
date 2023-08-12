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
    [HideInInspector] public float abilityLevel;
    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float attackSize;
    [HideInInspector] public int attackCount;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float knockbackPower;
    [HideInInspector] public float DoT;

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

        if (attackDamage <= float.Epsilon)
        {
            yield return new WaitForSeconds(0.15f);
            this.gameObject.SetActive(false);
            yield break;
        }

        //targetPos = GameManager.instance.Player.transform.position;

        //float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);
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
                hits = Physics.OverlapSphere(transform.position, attackRange, layer.value);
                break;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent<Enemy>(out Enemy enemy))
            {
                //enemy.TakeDamage(damage);
                float armor = enemy.attributes.GetAttribute("Armor").Value;
                DamageCalculator.ApplyDamage(attackDamage, critRate, critDMG, armor, enemy);
                enemy.ApplyDoT(DoT, 4f, 2f + attackCount);
                if (enemy.TryGetComponent<ImpactReceiver>(out ImpactReceiver impact))
                {
                    Vector3 dir = (enemy.transform.position - transform.position).normalized;
                    impact.AddImpact(dir, -knockbackPower);
                }
            }
        }

        yield return new WaitForSeconds(0.15f);
        this.gameObject.SetActive(false);
        yield break;
    }
}
