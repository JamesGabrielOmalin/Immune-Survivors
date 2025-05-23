using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Macrophage_BasicAttack", menuName = "Ability System/Abilities/Macrophage Basic Attack")]
public class Macrophage_BasicAttack : Ability
{
    public GameObject attackPrefab;

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Macrophage_BasicAttackSpec(this, owner);
        return spec;
    }
}

public class Macrophage_BasicAttackSpec : AbilitySpec
{
    private Macrophage_BasicAttack basicAttack;

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

    // constructor
    public Macrophage_BasicAttackSpec(Macrophage_BasicAttack ability, AbilitySystem owner) : base(ability, owner)
    {
        //InitializeAttributes(owner.gameObject);
        Init();
        owner.StartCoroutine(TryActivateAbility());
    }

    public bool IsAttacking { get; private set; } = false;

    public override bool CanActivateAbility()
    {
        return base.CanActivateAbility() && !IsAttacking;
    }

    private static readonly WaitForSeconds attackWait = new(1f);

    public override IEnumerator ActivateAbility()
    {
        while (true)
        {
            IsAttacking = true;

            // Wait before consuming
            yield return new WaitForSeconds(1f / attackSpeed.Value);

            // start consume
            if (owner.GetComponent<AbilitySet>().CanUseBasicAttack)
                Consume();

            yield return attackWait;
        }
    }

    public override void EndAbility()
    {
        IsAttacking = false;
        base.EndAbility();
    }

    private void Consume()
    {
        // implement basic shooting towards target
        GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, attackRange.Value);
        if (target == null)
        {
            return;
        }

        var basicAttack = ability as Macrophage_BasicAttack;

        GameObject consume = GameObject.Instantiate(basicAttack.attackPrefab, owner.transform.position, Quaternion.identity, owner.transform);
        MacrophageConsume consumeComp = consume.GetComponent<MacrophageConsume>();

        // Set attributes
        consumeComp.attackDamage = attackDamage.Value;
        consumeComp.attackRange = attackRange.Value;
        consumeComp.attackSize = attackSize.Value;
        consumeComp.critRate = critRate.Value;
        consumeComp.critDMG = critDMG.Value;
        consumeComp.knockbackPower = knockbackPower.Value;
        consumeComp.dot = attackDamage.Value / 4f;
        consumeComp.duration = 5f;
        consumeComp.tickRate = (int)attackCount.Value;
        consumeComp.Type_1_DMG_Bonus = Type_1_DMG_Bonus.Value;
        consumeComp.Type_2_DMG_Bonus = Type_2_DMG_Bonus.Value;
        consumeComp.Type_3_DMG_Bonus = Type_3_DMG_Bonus.Value;

        consumeComp.transform.localScale = Vector3.one * attackSize.Value;
    }

    // Cache all attributes required by this ability
    public void Init()
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

        basicAttack = ability as Macrophage_BasicAttack;
    }
}