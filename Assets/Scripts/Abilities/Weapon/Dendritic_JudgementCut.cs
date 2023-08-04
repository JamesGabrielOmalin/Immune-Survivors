using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dendritic_JudgementCut", menuName = "Ability System/Abilities/Dendritic Judgement Cut")]
public class Dendritic_JudgementCut : Ability
{
    [field:SerializeField] public float AttackDamageScaling { get; private set; }
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
    #endregion Attributes

    private ObjectPool cuts;

    // constructor
    public Dendritic_JudgementCutSpec(Dendritic_JudgementCut ability, AbilitySystem owner) : base(ability, owner)
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
            yield return Slash();

        yield break;
    }

    public override void EndAbility()
    {
        IsAttacking = false;
        base.EndAbility();
    }

    private IEnumerator Slash()
    {
        WaitForSeconds wait = new(0.25f);

        int count = (int)attackCount.Value;
        float AD = attackDamage.Value * basicAttack.AttackDamageScaling;
        float CRIT_RATE = critRate.Value;
        float CRIT_DMG = critDMG.Value;
        float SIZE = attackSize.Value;

        for (int i = 0; i < count; i++)
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

        basicAttack = ability as Dendritic_JudgementCut;

        cuts = GameObject.Find("Dendritic Judgement Cut Pool").GetComponentInChildren<ObjectPool>();
    }
}