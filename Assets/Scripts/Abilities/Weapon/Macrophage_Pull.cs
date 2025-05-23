using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Macrophage_Pull", menuName = "Ability System/Abilities/Macrophage Pull")]
public class Macrophage_Pull : Ability
{
    [field: SerializeField] public float AttackInterval { get; private set; }
    [field: SerializeField] public float KnockbackPower { get; private set; }
    [field: SerializeField] public MacrophagePullType PullType { get; private set; }

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        return new Macrophage_PullSpec(this, owner);
    }
}

public class Macrophage_PullSpec : AbilitySpec
{
    private Macrophage_Pull basicAttack;

    #region Attributes
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
    public Attribute Type_1_DMG_Bonus;
    public Attribute Type_2_DMG_Bonus;
    public Attribute Type_3_DMG_Bonus;

    #endregion Attributes

    private ObjectPool pulls;

    public Macrophage_PullSpec(Macrophage_Pull ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
        owner.StartCoroutine(TryActivateAbility());
    }

    public override IEnumerator ActivateAbility()
    {
        while (true)
        {
            yield return new WaitForSeconds(basicAttack.AttackInterval / attackSpeed.Value);

            if (owner.GetComponent<AbilitySet>().CanUseBasicAttack)
                Pull();
        }
    }

    private void Pull()
    {
        float AD = attackDamage.Value;
        float AR = attackRange.Value;

        GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, AR);
        if (target == null)
        {
            return;
        }

        Vector3 dir = (target.transform.position - owner.transform.position).normalized;

        GameObject pull = pulls.RequestPoolable(owner.transform.position);

                                    // Level 2 and higher: Increase Size by 50%
        float AZ = attackSize.Value * (abilityLevel >= 2f ? 1.5f : 1f);

        MacrophagePull pullEffect = pull.GetComponent<MacrophagePull>();
        pullEffect.abilityLevel = abilityLevel;
        pullEffect.attackDamage = AD;
        pullEffect.attackRange = AR;
                                     // Level 4 and higher: Increase DoT by 5
        pullEffect.DoT = (AD / 4f) + (abilityLevel >= 4f ? 5f : 0f);
                                                           // Level 3 and higher: Increase DoT tick count by 4
        pullEffect.attackCount = (int)attackCount.Value + (abilityLevel >= 3f ? 4 : 0);
        pullEffect.attackSize = AZ;
        pullEffect.critRate = critRate.Value;
        pullEffect.critDMG = critDMG.Value;

        pullEffect.knockbackPower = (basicAttack.KnockbackPower + knockbackPower.Value);
        // Increase knockback by 25% per level
        pullEffect.knockbackPower += pullEffect.knockbackPower * ((abilityLevel - 1) * 1.25f);

        pullEffect.transform.localScale = Vector3.one * AZ;

        pullEffect.Type_1_DMG_Bonus = Type_1_DMG_Bonus.Value;
        pullEffect.Type_2_DMG_Bonus = Type_2_DMG_Bonus.Value;
        pullEffect.Type_3_DMG_Bonus = Type_3_DMG_Bonus.Value;

        switch (basicAttack.PullType)
        {
            case MacrophagePullType.Line:
                pullEffect.transform.forward = dir;
                AudioManager.instance.Play("MacrophageLine", owner.transform.position);
                break;
            case MacrophagePullType.Cone:
                pullEffect.transform.forward = dir;
                pull.transform.position = target.transform.position;
                AudioManager.instance.Play("MacrophageCone", owner.transform.position);
                break;
            case MacrophagePullType.Circle:
                AudioManager.instance.Play("MacrophageCircle", owner.transform.position);
                break;
        }

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
        knockbackPower = attributes.GetAttribute(Attribute.KNOCKBACK_POWER);

        Type_1_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_1_DMG_BONUS);
        Type_2_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_2_DMG_BONUS);
        Type_3_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_3_DMG_BONUS);

        basicAttack = ability as Macrophage_Pull;
        pulls = GameObject.Find("Macrophage " + basicAttack.PullType.ToString() + " Pool").GetComponentInChildren<ObjectPool>();
    }
}
