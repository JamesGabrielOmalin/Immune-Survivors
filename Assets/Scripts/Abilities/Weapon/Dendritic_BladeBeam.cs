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
            IsAttacking = true;

            // Wait before shooting                                                          // Level 3 and higher: Increase ATK SPD by 20%
            yield return new WaitForSeconds(basicAttack.AttackInterval / attackSpeed.Value * (abilityLevel >= 3f ? 1.2f : 1f));

            // start slashing
            if (owner.GetComponent<AbilitySet>().CanUseBasicAttack)
            {
                AudioManager.instance.Play("DentriticBlade", owner.transform.position);
                yield return Slash();
            }
                
        }
    }

    public override void EndAbility()
    {
        IsAttacking = false;
        base.EndAbility();
    }

    private IEnumerator Slash()
    {
        WaitForSeconds wait = new(0.25f);

        float AD = attackDamage.Value;
                                        // Level 4 or higher: Increase range by 25%
        float AR = attackRange.Value * (abilityLevel >= 4f ? 1.25f : 1f);
                                            // Increase CRIT Rate by 10% per level
        float CRIT_RATE = critRate.Value + ((abilityLevel - 1) * 0.1f);
        float CRIT_DMG = critDMG.Value;
                                        // Level 2 or higher: Increase size by 50%
        float AZ = attackSize.Value * (abilityLevel >= 2f ? 1.5f : 1f);
        Vector3 scale = Vector3.one * AZ;

        int AC = (int)attackCount.Value;

        for (int i = 0; i < AC; i++)
        {
            // implement basic shooting towards target
            GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, AR);
            if (target == null)
            {
                continue;
            }

            Vector3 dir = (target.transform.position - owner.transform.position).normalized;

            GameObject projectile = bladeBeams.RequestPoolable(owner.transform.position);
            if (projectile == null)
                continue;
            DendriticBladeBeam cut = projectile.GetComponent<DendriticBladeBeam>();
            cut.transform.forward = dir;

            // Snapshot attributes
            cut.attackDamage = AD;
            cut.critRate = CRIT_RATE;
            cut.critDMG = CRIT_DMG;
            cut.attackRange = AR;

            cut.transform.localScale = scale;

            yield return wait;
        }
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
