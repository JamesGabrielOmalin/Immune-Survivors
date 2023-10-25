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
    public Attribute Type_1_DMG_Bonus;
    public Attribute Type_2_DMG_Bonus;
    public Attribute Type_3_DMG_Bonus;
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
            yield return new WaitForSeconds(basicAttack.AttackInterval / (attackSpeed.Value * (abilityLevel >= 3f ? 1.2f : 1f)));

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
                                                                // Level 3 and higher: Increase ATK SPD by 20%
        WaitForSeconds wait = new(0.25f / (attackSpeed.Value * (abilityLevel >= 3f ? 1.2f : 1f)));

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

        float Type_1 = Type_1_DMG_Bonus.Value;
        float Type_2 = Type_2_DMG_Bonus.Value;
        float Type_3 = Type_3_DMG_Bonus.Value;

        Vector3 dir = Vector3.forward;
        for (int i = 0; i < AC; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                switch (j)
                {
                    // If odd cycle, + pattern. Otherwise, x pattern
                    case 0:
                        dir = i % 2 == 0 ? Vector3.forward : (Vector3.forward + Vector3.right).normalized;
                        break;
                    case 1:
                        dir = i % 2 == 0 ? Vector3.right : (Vector3.right + Vector3.back).normalized;
                        break;
                    case 2:
                        dir = i % 2 == 0 ? Vector3.back : (Vector3.back + Vector3.left).normalized;
                        break;
                    case 3:
                        dir = i % 2 == 0 ? Vector3.left : (Vector3.left + Vector3.forward).normalized;
                        break;
                }

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
                cut.Type_1_DMG_Bonus = Type_1;
                cut.Type_2_DMG_Bonus = Type_2;
                cut.Type_3_DMG_Bonus = Type_3;

                cut.transform.localScale = scale;

                yield return wait;
            }
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

        Type_1_DMG_Bonus = attributes.GetAttribute("Type_1 DMG Bonus");
        Type_2_DMG_Bonus = attributes.GetAttribute("Type_2 DMG Bonus");
        Type_3_DMG_Bonus = attributes.GetAttribute("Type_3 DMG Bonus");

        basicAttack = ability as Dendritic_BladeBeam;

        bladeBeams = GameObject.Find("Dendritic Blade Beam Pool").GetComponentInChildren<ObjectPool>();
    }
}
