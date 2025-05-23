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
    public Attribute Type_1_DMG_Bonus;
    public Attribute Type_2_DMG_Bonus;
    public Attribute Type_3_DMG_Bonus;

    private Neutrophil_Stab basicAttack;

    private ObjectPool stabs;

    public Neutrophil_StabSpec(Neutrophil_Stab ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
        owner.StartCoroutine(TryActivateAbility());
    }

    public bool IsAttacking { get; private set; } = false;

    public override bool CanActivateAbility()
    {
        return base.CanActivateAbility() && !IsAttacking;
    }

    public override IEnumerator ActivateAbility()
    {
        while (true)
        {
            if (!owner.GetComponent<AbilitySet>().CanUseBasicAttack)
                yield break;

            IsAttacking = true;

            // Increase attack speed per level
            float AS = attackSpeed.Value + ((abilityLevel - 1) * 1.1f);
            yield return new WaitForSeconds(2.5f / AS);

            Stab();
        }
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
        
        AudioManager.instance.Play("NeutrophilStab", owner.transform.position);
        stabObject.transform.forward = dir;

        NeutrophilStab stab = stabObject.GetComponent<NeutrophilStab>();
        stab.target = target.GetComponent<Enemy>();
        stab.attackDamage = AD;
        stab.attackCount = AC;
        stab.critRate = CRIT_RATE;
        stab.critDMG = CRIT_DMG;
                                // Level 4 and higher: Increase DoT by 5
        stab.DoT = (AD / 4f) + (abilityLevel >= 4 ? 5f : 0f);

        stab.Type_1_DMG_Bonus = Type_1_DMG_Bonus.Value;
        stab.Type_2_DMG_Bonus = Type_2_DMG_Bonus.Value;
        stab.Type_3_DMG_Bonus = Type_3_DMG_Bonus.Value;

        stab.transform.localScale = scale;
        Debug.Log("STAB STAB STAB");
    }

    private void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();

        level = attributes.GetAttribute(Attribute.LEVEL);
        attackDamage = attributes.GetAttribute(Attribute.ATTACK_DAMAGE);
        attackSpeed = attributes.GetAttribute(Attribute.ATTACK_SPEED);
        attackRange = attributes.GetAttribute(Attribute.ATTACK_RANGE);
        attackCount = attributes.GetAttribute(Attribute.ATTACK_COUNT);
        attackSize = attributes.GetAttribute(Attribute.ATTACK_SIZE);
        critRate = attributes.GetAttribute(Attribute.CRITICAL_RATE);
        critDMG = attributes.GetAttribute(Attribute.CRITICAL_DAMAGE);

        Type_1_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_1_DMG_BONUS);
        Type_2_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_2_DMG_BONUS);
        Type_3_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_3_DMG_BONUS);

        basicAttack = ability as Neutrophil_Stab;

        stabs = GameObject.Find("Neutrophil Stab Pool").GetComponentInChildren<ObjectPool>();
    }
}