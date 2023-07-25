using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dendritic_BladeBeam", menuName = "Ability System/Abilities/Dendritic Blade Beam")]
public class Dendritic_BladeBeam : Ability
{
    [field: SerializeField] public float AttackInterval { get; private set; }

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Dendritic_BladeBeamSpec(this, owner);
        return spec;
    }
}

public class Dendritic_BladeBeamSpec : AbilitySpec
{
    private Dendritic_BladeBeam basicAttack;

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

    private ObjectPool bladeBeams;

    public Dendritic_BladeBeamSpec(Dendritic_BladeBeam ability, AbilitySystem owner) : base(ability, owner)
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
        yield return new WaitForSeconds(basicAttack.AttackInterval / attackSpeed.Value);

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
        float range = attackRange.Value;
        // implement basic shooting towards target
        GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, range);
        if (target == null)
        {
            return;
        }

        Vector3 dir = (target.transform.position - owner.transform.position).normalized;

        GameObject projectile = bladeBeams.RequestPoolable(owner.transform.position);
        if (projectile == null)
            return;
        DendriticBladeBeam cut = projectile.GetComponent<DendriticBladeBeam>();
        cut.transform.forward = dir;

        // Snapshot attributes
        cut.attackDamage = attackDamage.Value;
        cut.critRate = critRate.Value;
        cut.critDMG = critDMG.Value;
        cut.attackCount = (int)attackCount.Value;
        cut.attackRange = range;

        cut.transform.localScale = Vector3.one * attackSize.Value;
    }

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

        basicAttack = ability as Dendritic_BladeBeam;

        bladeBeams = GameObject.Find("Dendritic Blade Beam Pool").GetComponentInChildren<ObjectPool>();
    }
}
