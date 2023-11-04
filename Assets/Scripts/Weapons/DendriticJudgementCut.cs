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
    [HideInInspector] public float Type_1_DMG_Bonus;
    [HideInInspector] public float Type_2_DMG_Bonus;
    [HideInInspector] public float Type_3_DMG_Bonus;

    private const float DROP_RATE = 0.33333f;

    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(Slash());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private readonly WaitForSeconds wait = new(1.5f);
    private IEnumerator Slash()
    {
        vfx.Play();

        var hits = Physics.OverlapSphere(transform.position, attackSize * 2.5f, layerMask.value);
        //float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                float MaxHP = enemy.MaxHP.Value;
                float HP = enemy.HP.Value;

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

                float ratio = Mathf.SmoothStep(0.25f, 0.75f, (HP / MaxHP));
                float missingHPBonusDMG = Mathf.Lerp(1f, 0.1f, ratio);

                float damage = attackDamage * DMGBonus * missingHPBonusDMG;
                float armor = enemy.Armor.Value;
                DamageCalculator.ApplyDamage(damage, critRate, critDMG, armor, enemy);

                if (enemy.IsDead)
                {
                    if (Random.value <= DROP_RATE)
                    {
                        AudioManager.instance.Play("PlayerPickUp", transform.position);
                        AntigenManager.instance.AddBonusAntigen(enemy.Type);
                    }
                }
                //enemy.TakeDamage(damage);
            }
        }

        yield return wait;
        this.gameObject.SetActive(false);
        yield break;
    }
}
