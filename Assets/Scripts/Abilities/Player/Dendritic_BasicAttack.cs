using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dendritic_BasicAttack", menuName = "Ability System/Abilities/Dendritic Basic Attack")]
public class Dendritic_BasicAttack : Ability
{
    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Dendritic_BasicAttackSpec(this, owner);
        return spec;
    }
}

public class Dendritic_BasicAttackSpec : AbilitySpec
{
    private Dendritic_BasicAttack basicAttack;

    #region Attributes
    public AttributeSet attributes;

    public Attribute level;
    public Attribute attackDamage;
    public Attribute attackSpeed;
    public Attribute attackRange;
    public Attribute attackCount;
    public Attribute attackSize;
    public Attribute critRate;
    public Attribute critDMG;
    #endregion Attributes

    private ObjectPool slashes;

    // constructor
    public Dendritic_BasicAttackSpec(Dendritic_BasicAttack ability, AbilitySystem owner) : base(ability, owner)
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
        IsAttacking = true;

        // Wait before shooting
        yield return new WaitForSeconds(1 / attackSpeed.Value);

        // start slashing
        if (owner.GetComponent<AbilitySet>().CanUseBasicAttack)
            Slash();

        yield break;
    }

    public override void EndAbility()
    {
        IsAttacking = false;
        base.EndAbility();
    }

    private void Slash()
    {
        // implement basic shooting towards target
        GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, attackRange.Value);
        if (target == null)
        {
            return;
        }

        GameObject projectile = slashes.RequestPoolable(target.transform.position);
        if (projectile == null)
            return;

        DendriticSlash slash = projectile.GetComponent<DendriticSlash>();
        slash.target = target.GetComponent<Enemy>();

        if (!slash.target)
        {
            Debug.LogError("WHAT?");
        }

        // Snapshot attributes
        slash.attackDamage = attackDamage.Value;
        slash.critRate = critRate.Value;
        slash.critDMG = critDMG.Value;
        slash.attackCount = (int)attackCount.Value + 4;
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
        attackCount = attributes.GetAttribute("Attack Count");
        attackSize = attributes.GetAttribute("Attack Size");

        basicAttack = ability as Dendritic_BasicAttack;

        slashes = GameObject.Find("Dendritic Slash Pool").GetComponentInChildren<ObjectPool>();
    }
}