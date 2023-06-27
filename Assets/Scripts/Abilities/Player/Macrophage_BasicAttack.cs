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
    public Attribute critRate;
    public Attribute critDMG;
    public Attribute knockbackPower;
    public Attribute dotAmount;
    public Attribute dotDuration;
    public Attribute dotTickRate;

    #endregion Attributes

    // constructor
    public Macrophage_BasicAttackSpec(Macrophage_BasicAttack ability, AbilitySystem owner) : base(ability, owner)
    {
        //InitializeAttributes(owner.gameObject);
        Init();
    }

    public bool IsAttacking { get; private set; } = false;

    public override bool CanActivateAbility()
    {
        return base.CanActivateAbility() && !IsAttacking;
    }

    public override IEnumerator ActivateAbility()
    {
        IsAttacking = true;

        // Wait before consuming
        yield return new WaitForSeconds(1 / attackSpeed.Value);

        // start consume
        Consume();

        yield return new WaitForSeconds(1f);

        yield break;
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

        Vector3 dir = Vector3.Normalize(target.transform.position - owner.transform.position);

        GameObject consume = GameObject.Instantiate(basicAttack.attackPrefab, owner.transform.position + dir, Quaternion.identity, owner.transform);
        MacrophageConsume consumeComp = consume.GetComponent<MacrophageConsume>();

        // Set attributes
        consume.name = consumeComp.name + "(0)";
        consumeComp.attackDamage = attackDamage.Value;
        consumeComp.attackRange = attackRange.Value;
        consumeComp.critRate = critRate.Value;
        consumeComp.critDMG = critDMG.Value;
        consumeComp.knockbackPower = knockbackPower.Value;
        consumeComp.dot = dotAmount.Value;
        consumeComp.duration = dotDuration.Value;
        consumeComp.tickRate = dotTickRate.Value;
    }

    // Cache all attributes required by this ability
    public void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();

        level = attributes.GetAttribute("Level");
        attackDamage = attributes.GetAttribute("Attack Damage");
        critRate = attributes.GetAttribute("Critical Rate");
        critDMG = attributes.GetAttribute("Critical Damage");
        attackSpeed = attributes.GetAttribute("Attack Speed");
        attackRange = attributes.GetAttribute("Attack Range");
        knockbackPower = attributes.GetAttribute("Knockback Power");
        dotAmount = attributes.GetAttribute("DoT Amount");
        dotDuration = attributes.GetAttribute("DoT Duration");
        dotTickRate = attributes.GetAttribute("DoT Tick Rate");

        basicAttack = ability as Macrophage_BasicAttack;
    }
}