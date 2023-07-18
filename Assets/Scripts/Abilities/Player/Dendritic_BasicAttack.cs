using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dendritic_BasicAttack", menuName = "Ability System/Abilities/Dendritic Basic Attack")]
public class Dendritic_BasicAttack : Ability
{
    public GameObject projectilePrefab;

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
    public Attribute knockbackPower;
    #endregion Attributes

    // constructor
    public Dendritic_BasicAttackSpec(Dendritic_BasicAttack ability, AbilitySystem owner) : base(ability, owner)
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

        Vector3 targetPos = target.transform.position;
        Vector3 newTargetPos = targetPos;
        newTargetPos.y = owner.transform.position.y;

        GameObject projectile = GameObject.Instantiate(basicAttack.projectilePrefab, target.transform.position, Quaternion.identity);
        DendriticSlash slash = projectile.GetComponent<DendriticSlash>();

        // Snapshot attributes
        slash.attackDamage = attackDamage.Value;
        slash.critRate = critRate.Value;
        slash.critDMG = critDMG.Value;
        slash.slashCount = (int)attackCount.Value;
        slash.transform.localScale = Vector3.one * attackSize.Value;
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
        attackCount = attributes.GetAttribute("Attack Count");
        attackSize = attributes.GetAttribute("Attack Size");

        basicAttack = ability as Dendritic_BasicAttack;
    }
}