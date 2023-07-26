using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutrophilGrenade : Projectile
{
    [SerializeField] private GameObject slowField;
    [SerializeField] private LayerMask layerMask;

    private float t = 0f;
    private Vector3 startPos;
    [HideInInspector] public Vector3 targetPos;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackSize;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public float slowAmount;

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (t >= 1f)
            return;

        Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
        newPos.y = Mathf.Sin(t * 180f * Mathf.Deg2Rad);

        transform.position = newPos;

        t += Time.fixedDeltaTime * projectileSpeed;
    }

    private IEnumerator ActivateSlowField()
    {
        yield return new WaitForSeconds(1f / projectileSpeed);

        var hits = Physics.OverlapSphere(transform.position, attackSize, layerMask.value);

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);
            enemy.TakeDamage(damage);
        }

        slowField.SetActive(true);
        slowField.GetComponent<NeutrophilSlowField>().slowAmount = slowAmount;

        yield return LifeSpanCoroutine();
    }

    protected override void OnEnable()
    {
        startPos = transform.position;
        StartCoroutine(ActivateSlowField());
    }

    private void OnDisable()
    {
        t = 0f;
        slowField.SetActive(false);
        StopAllCoroutines();
    }
}
