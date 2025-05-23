using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dendritic_BasicAttack", menuName = "Ability System/Abilities/Dendritic Basic Attack")]
public class Dendritic_BasicAttack : Ability
{
    [field: SerializeField] public float AttackDamageScaling { get; private set; }
    [field: SerializeField] public int AttackCount { get; private set; }
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
    public Attribute Type_1_DMG_Bonus;
    public Attribute Type_2_DMG_Bonus;
    public Attribute Type_3_DMG_Bonus;
    #endregion Attributes

    private ObjectPool slashes;

    // constructor
    public Dendritic_BasicAttackSpec(Dendritic_BasicAttack ability, AbilitySystem owner) : base(ability, owner)
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

            float interval = Mathf.Pow((float)System.Math.E, -attackSpeed.Value);

            // Wait before shooting
            yield return new WaitForSeconds(interval);

            // start slashing
            if (owner.GetComponent<AbilitySet>().CanUseBasicAttack)
                yield return Slash();
        }
    }

    public override void EndAbility()
    {
        IsAttacking = false;
        base.EndAbility();
    }

    private IEnumerator Slash()
    {
        WaitForSeconds wait = new(0.15f);
                                                                            // Level 4 and higher: Increase DMG by 5
        float AD = (attackDamage.Value * basicAttack.AttackDamageScaling) + (abilityLevel >= 4f ? 5f : 0f);
                                            // Increase CRIT Rate by 10% per level
        float CRIT_RATE = critRate.Value + ((abilityLevel - 1) * 0.1f);
                                            // Level 2 and higher: Increase CRIT DMG by 25%
        float CRIT_DMG = critDMG.Value + (abilityLevel >= 2f ? 0.25f : 0f);
        int AC = (int)attackCount.Value;
                                            // Level 3 and higher: Increase slash count by 3
        int count = basicAttack.AttackCount + (abilityLevel >= 3f ? 3 : 0);

        float Type_1 = Type_1_DMG_Bonus.Value;
        float Type_2 = Type_2_DMG_Bonus.Value;
        float Type_3 = Type_3_DMG_Bonus.Value;

        // implement basic shooting towards target
        for (int i = 0; i < AC; i++)
        {
            GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, attackRange.Value);
            if (target == null)
            {
                continue;
            }

            GameObject projectile = slashes.RequestPoolable(target.transform.position);
            if (projectile == null)
                continue;

            DendriticSlash slash = projectile.GetComponent<DendriticSlash>();
            slash.target = target.GetComponent<Enemy>();

            // Snapshot attributes
            slash.attackDamage = AD;
            slash.critRate = CRIT_RATE;
            slash.critDMG = CRIT_DMG;
            slash.attackCount = count;
            slash.Type_1_DMG_Bonus = Type_1;
            slash.Type_2_DMG_Bonus = Type_2;
            slash.Type_3_DMG_Bonus = Type_3;

            AudioManager.instance.Play("DentriticSlash", slash.transform.position);

            yield return wait;
        }
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

        Type_1_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_1_DMG_BONUS);
        Type_2_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_2_DMG_BONUS);
        Type_3_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_3_DMG_BONUS);

        basicAttack = ability as Dendritic_BasicAttack;

        slashes = GameObject.Find("Dendritic Slash Pool").GetComponentInChildren<ObjectPool>();
    }
}