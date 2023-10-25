using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dendritic_JudgementCut", menuName = "Ability System/Abilities/Dendritic Judgement Cut")]
public class Dendritic_JudgementCut : Ability
{
    [field:SerializeField] public float AttackDamageScaling { get; private set; }
    [field:SerializeField] public float AttackInterval { get; private set; }
    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Dendritic_JudgementCutSpec(this, owner);
        return spec;
    }
}

public class Dendritic_JudgementCutSpec : AbilitySpec
{
    private Dendritic_JudgementCut basicAttack;

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

    private ObjectPool cuts;

    // constructor
    public Dendritic_JudgementCutSpec(Dendritic_JudgementCut ability, AbilitySystem owner) : base(ability, owner)
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

            // Wait before shooting                                                            // Level 4 and higher: Increase ATK SPD by 30%
            yield return new WaitForSeconds(basicAttack.AttackInterval / (attackSpeed.Value * (abilityLevel >= 4 ? 1.3f : 1f)));

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
        WaitForSeconds wait = new(0.25f);
                                           
        int AC = (int)attackCount.Value;
        float AD = attackDamage.Value * basicAttack.AttackDamageScaling;
                                            // Increase CRIT Rate by 10% per level
        float CRIT_RATE = critRate.Value + ((abilityLevel - 1) * 0.1f);
                                          // Level 3 and higher: Increase CRIT DMG by 50%
        float CRIT_DMG = critDMG.Value + (abilityLevel >= 3 ? 0.5f : 0f);
                                         // Level 2 and higher: Increase size by 15%
        float SIZE = attackSize.Value * (abilityLevel >= 2 ? 1.15f : 1f);

        AudioManager.instance.Play("DentriticJN", owner.transform.position);
        for (int i = 0; i < AC; i++)
        {
            // implement basic shooting towards target
            GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, attackRange.Value);
            if (target == null)
            {
                continue;
            }

            GameObject projectile = cuts.RequestPoolable(target.transform.position);
            if (projectile == null)
                continue;
            DendriticJudgementCut cut = projectile.GetComponent<DendriticJudgementCut>();

            // Snapshot attributes
            cut.attackDamage = AD;
            cut.critRate = CRIT_RATE;
            cut.critDMG = CRIT_DMG;
            cut.attackSize = SIZE;

            cut.transform.localScale = Vector3.one * SIZE;

            yield return wait;
        }
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

        Type_1_DMG_Bonus = attributes.GetAttribute("Type_1 DMG Bonus");
        Type_2_DMG_Bonus = attributes.GetAttribute("Type_2 DMG Bonus");
        Type_3_DMG_Bonus = attributes.GetAttribute("Type_3 DMG Bonus");

        basicAttack = ability as Dendritic_JudgementCut;

        cuts = GameObject.Find("Dendritic Judgement Cut Pool").GetComponentInChildren<ObjectPool>();
    }
}