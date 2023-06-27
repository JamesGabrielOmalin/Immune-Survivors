using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Neutrophil_Ultimate", menuName = "Ability System/Abilities/Neutrophil Ultimate")]
public class Neutrophil_Ultimate : Ability
{
    [SerializeField] public GameObject ultimateVFX;

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Neutrophil_UltimateSpec(this, owner);
        return spec;
    }
}

public class Neutrophil_UltimateSpec : AbilitySpec
{
    public AttributeSet attributes;
    public Attribute level; 
    public Attribute attackDamage;
    public Attribute critRate;
    public Attribute critDMG;
    public Attribute knockbackPower;

    private Neutrophil_Ultimate ult;

    public Neutrophil_UltimateSpec(Neutrophil_Ultimate ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
    }

    private GameObject vfxInstance = null;

    // TODO: Make required level visible on ScriptableObject
    public override bool CanActivateAbility()
    {
        return level.Value >= 6f && base.CanActivateAbility();
    }

    public override IEnumerator ActivateAbility()
    {
        WaitForSeconds attackInterval = new(0.125f);

        float ad = attackDamage.Value;
        float cr = critRate.Value;
        float cd = critDMG.Value;
        float knockBack = knockbackPower.Value;

        vfxInstance = GameObject.Instantiate(ult.ultimateVFX, owner.transform);
        vfxInstance.GetComponent<VisualEffect>().Play();

        for (int i = 0; i < 20; i++)
        {
            var hits = Physics.OverlapSphere(owner.transform.position, 5f);

            foreach (var hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (!enemy)
                    continue;

                Vector3 dir = (enemy.transform.position - owner.transform.position).normalized;

                enemy.TakeDamage(DamageCalculator.CalcDamage(ad, cr, cd));

                // 25% chance to apply knockback every other hits
                if (i % 2 == 0 && Random.value <= 0.25f)
                    enemy.GetComponent<ImpactReceiver>().AddImpact(dir, knockBack * 2.5f);
            }

            yield return attackInterval;
        }

        vfxInstance.GetComponent<VisualEffect>().Stop();

        CurrentCD = ability.Cooldown;
        owner.StartCoroutine(UpdateCD());

        yield return new WaitForSeconds(1f);
    }

    public override void EndAbility()
    {
        GameObject.Destroy(vfxInstance);
        vfxInstance = null;
        base.EndAbility();
    }

    public void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();

        level = attributes.GetAttribute("Level");
        attackDamage = owner.GetComponent<AttributeSet>().GetAttribute("Attack Damage");
        critRate = owner.GetComponent<AttributeSet>().GetAttribute("Critical Rate");
        critDMG = owner.GetComponent<AttributeSet>().GetAttribute("Critical Damage");
        knockbackPower = owner.GetComponent<AttributeSet>().GetAttribute("Knockback Power");

        ult = ability as Neutrophil_Ultimate;
    }
}

