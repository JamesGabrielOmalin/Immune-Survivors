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

    #endregion Attributes

    private ObjectPool pulls;

    public Macrophage_PullSpec(Macrophage_Pull ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
    }

    public override IEnumerator ActivateAbility()
    {
        yield return new WaitForSeconds(basicAttack.AttackInterval / attackSpeed.Value);

        if (owner.GetComponent<AbilitySet>().CanUseBasicAttack)
            Pull();

        yield break;
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

        switch (basicAttack.PullType)
        {
            case MacrophagePullType.Line:
                pullEffect.transform.forward = dir;
                break;
            case MacrophagePullType.Cone:
                pullEffect.transform.forward = dir;
                pull.transform.position = target.transform.position;
                break;
            case MacrophagePullType.Circle:
                break;
        }
    }

    private void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();

        level = attributes.GetAttribute("Level");
        attackDamage = attributes.GetAttribute("Attack Damage");
        critRate = attributes.GetAttribute("Critical Rate");
        critDMG = attributes.GetAttribute("Critical Damage");
        attackSpeed = attributes.GetAttribute("Attack Speed");
        attackRange = attributes.GetAttribute("Attack Range");
        attackSize = attributes.GetAttribute("Attack Size");
        attackCount = attributes.GetAttribute("Attack Count");
        knockbackPower = attributes.GetAttribute("Knockback Power");

        basicAttack = ability as Macrophage_Pull;
        pulls = GameObject.Find("Macrophage " + basicAttack.PullType.ToString() + " Pool").GetComponentInChildren<ObjectPool>();
    }
}
