using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class NeutrophilGrenade : Projectile
{
    [SerializeField] private VisualEffect vfx;
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
    [HideInInspector] public float Type_1_DMG_Bonus;
    [HideInInspector] public float Type_2_DMG_Bonus;
    [HideInInspector] public float Type_3_DMG_Bonus;

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

        var hits = Physics.OverlapSphere(transform.position, attackSize + 1f, layerMask.value);

        foreach (var hit in hits)
        {
            //Enemy enemy = hit.GetComponent<Enemy>();

            if (hit.TryGetComponent(out Enemy enemy))
            {
                float DMGBonus = Type_1_DMG_Bonus;

                switch (enemy.Type)
                {
                    case AntigenType.Type_1:
                        DMGBonus = Type_1_DMG_Bonus;
                        break;
                    case AntigenType.Type_2:
                        DMGBonus = Type_2_DMG_Bonus;
                        break;
                    case AntigenType.Type_3:
                        DMGBonus = Type_3_DMG_Bonus;
                        break;
                }

                float damage = attackDamage * DMGBonus;
                float armor = enemy.Armor.Value;
                DamageCalculator.ApplyDamage(damage, critRate, critDMG, armor, enemy);
            }

            //float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);
            //enemy.TakeDamage(damage);
        }

        vfx.SetBool("Alive", false);
        vfx.SendEvent("Kill");

        slowField.SetActive(true);
        AudioManager.instance.Play("NeutrophilGrenade", transform.position);

        var field = slowField.GetComponent<NeutrophilSlowField>();

        field.slowAmount = slowAmount;
        field.duration = lifeSpan;

        yield return LifeSpanCoroutine();
    }

    protected override void OnEnable()
    {
        startPos = transform.position;
        vfx.SetBool("Alive", true);
        StartCoroutine(ActivateSlowField());
    }

    private void OnDisable()
    {
        t = 0f;
        slowField.SetActive(false);
        StopAllCoroutines();
    }
}
