using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Neutrophil_Stab", menuName = "Ability System/Abilities/Neutrophil Stab")]
public class Neutrophil_Stab : Ability
{
    [field: SerializeField] public float Range { get; private set; }

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        return new Neutrophil_StabSpec(this, owner);
    }
}

public class Neutrophil_StabSpec : AbilitySpec
{
    public AttributeSet attributes;

    public Attribute level;
    public Attribute attackDamage;
    public Attribute attackSpeed;
    public Attribute attackRange;
    public Attribute attackSize;
    public Attribute attackCount;
    public Attribute critRate;
    public Attribute critDMG;
    public Attribute knockbackPower;

    private Neutrophil_Stab basicAttack;

    private ObjectPool stabs;

    public Neutrophil_StabSpec(Neutrophil_Stab ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
    }

    public bool IsAttacking { get; private set; } = false;

    public override bool CanActivateAbility()
    {
        return base.CanActivateAbility() && !IsAttacking;
    }

    public override IEnumerator ActivateAbility()
    {
        if (!owner.GetComponent<AbilitySet>().CanUseBasicAttack)
            yield break;

        IsAttacking = true;

        // Increase attack speed per level
        float AS = attackSpeed.Value + ((abilityLevel - 1) * 1.1f);
        yield return new WaitForSeconds(2.5f / AS);

        Stab();
    }

    public override void EndAbility()
    {
        IsAttacking = false;
        base.EndAbility();
    }

    private void Stab()
    {
        float AR = basicAttack.Range;
        int AC = (int)attackCount.Value;
                                        // Level 3 and higher: Increase DMG by 10
        float AD = attackDamage.Value + (abilityLevel >= 3 ? 10 : 0f);
        float AZ = attackSize.Value;
                                             // Level 2 and higher: Increase CRIT Rate by 10%
        float CRIT_RATE = critRate.Value + (abilityLevel >= 2 ? 1.1f : 0f);
        float CRIT_DMG = critDMG.Value;

        Vector3 scale = Vector3.one * AZ;

        GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, AR);

        if (target == null)
        {
            return;
        }

        Vector3 dir = target.transform.position - owner.transform.position;
        GameObject stabObject = stabs.RequestPoolable(target.transform.position);

        if (stabObject == null)
        {
            return;
        }

        stabObject.transform.forward = dir;

        NeutrophilStab stab = stabObject.GetComponent<NeutrophilStab>();
        stab.attackDamage = AD;
        stab.attackCount = AC;
        stab.critRate = CRIT_RATE;
        stab.critDMG = CRIT_DMG;
                                // Level 4 and higher: Increase DoT by 5
        stab.DoT = (AD / 4f) + (abilityLevel >= 4 ? 5f : 0f);

        stab.target = target.GetComponent<Enemy>();

        stab.transform.localScale = scale;
    }

    private void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();

        level = attributes.GetAttribute("Level");
        attackDamage = attributes.GetAttribute("Attack Damage");
        critRate = attributes.GetAttribute("Critical Rate");
        critDMG = attributes.GetAttribute("Critical Damage");
        attackSpeed = attributes.GetAttribute("Attack Speed");
        attackRange = attributes.GetAttribute("Attack Range");
        attackCount = attributes.GetAttribute("Attack Count");
        attackSize = attributes.GetAttribute("Attack Size");
        knockbackPower = attributes.GetAttribute("Knockback Power");

        basicAttack = ability as Neutrophil_Stab;

        stabs = GameObject.Find("Neutrophil Stab Pool").GetComponentInChildren<ObjectPool>();
    }
}