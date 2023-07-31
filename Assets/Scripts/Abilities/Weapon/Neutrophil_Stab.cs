using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Neutrophil_Stab", menuName = "Ability System/Abilities/Neutrophil Stab")]
public class Neutrophil_Stab : Ability
{
    [field: SerializeField] public float Range { get; private set; }

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        return new Neutrophil_StabSpec(this, owner);
    }
}

public class Neutrophil_StabSpec : AbilitySpec
{
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

    private Neutrophil_Stab basicAttack;

    private ObjectPool stabs;

    public Neutrophil_StabSpec(Neutrophil_Stab ability, AbilitySystem owner) : base(ability, owner)
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
        yield return new WaitUntil(() => owner.GetComponent<AbilitySet>().CanUseBasicAttack);
        IsAttacking = true;

        yield return new WaitForSeconds(2.5f / attackSpeed.Value);

        Stab();
    }

    public override void EndAbility()
    {
        IsAttacking = false;
        base.EndAbility();
    }

    private void Stab()
    {
        float range = basicAttack.Range;
        GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, range);

        if (target == null)
        {
            return;
        }

        Vector3 dir = target.transform.position - owner.transform.position;
        GameObject stabObject = stabs.RequestPoolable(target.transform.position);

        if (stabObject == null)
        {
            return;
        }

        stabObject.transform.forward = dir;

        int count = (int)attackCount.Value;

        float ad = attackDamage.Value;
        float az = attackSize.Value;
        float cr = critRate.Value;
        float cd = critDMG.Value;


        NeutrophilStab stab = stabObject.GetComponent<NeutrophilStab>();
        stab.attackDamage = attackDamage.Value;
        stab.attackRange = attackRange.Value;
        stab.attackCount = attackCount.Value;
        stab.attackSize = attackSize.Value;
        stab.critRate = critRate.Value;
        stab.critDMG = critDMG.Value;

        stab.target = target.GetComponent<Enemy>();

        stab.transform.localScale = Vector3.one * az;
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
        attackCount = attributes.GetAttribute("Attack Count");
        attackSize = attributes.GetAttribute("Attack Size");
        knockbackPower = attributes.GetAttribute("Knockback Power");

        basicAttack = ability as Neutrophil_Stab;

        stabs = GameObject.Find("Neutrophil Stab Pool").GetComponentInChildren<ObjectPool>();
    }
}