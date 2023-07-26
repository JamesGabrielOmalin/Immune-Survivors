using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Neutrophil_Grenade", menuName = "Ability System/Abilities/Neutrophil Grenade")]
public class Neutrophil_Grenade : Ability
{
    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        return new Neutrophil_GrenadeSpec(this, owner);
    }
}

public class Neutrophil_GrenadeSpec : AbilitySpec
{
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

    private Neutrophil_Grenade basicAttack;

    private ObjectPool grenades;

    public Neutrophil_GrenadeSpec(Neutrophil_Grenade ability, AbilitySystem owner) : base(ability, owner)
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

        // Wait before shooting
        yield return new WaitForSeconds(2f / attackSpeed.Value);
        
        Shoot();

        yield break;
    }

    private void Shoot()
    {
        GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, attackRange.Value / 2f);
        if (target == null)
        {
            return;
        }

        SpawnProjectile(target);
    }

    private void SpawnProjectile(in GameObject target)
    {
        GameObject grenadeObject = grenades.RequestPoolable(owner.transform.position);

        if (grenadeObject == null)
            return;

        Vector3 targetPos = target.transform.position;
        Vector3 dir = (targetPos - owner.transform.position).normalized;

        grenadeObject.transform.forward = dir;

        NeutrophilGrenade grenade = grenadeObject.GetComponent<NeutrophilGrenade>();
        grenade.targetPos = targetPos;
        grenade.attackDamage = attackDamage.Value;
        grenade.attackSize = attackSize.Value;
        grenade.critRate = critRate.Value;
        grenade.critDMG = critDMG.Value;
        grenade.slowAmount = -0.5f;

        grenade.transform.localScale = Vector3.one * attackSize.Value;
    }

    public override void EndAbility()
    {
        IsAttacking = false;
        base.EndAbility();
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
        knockbackPower = attributes.GetAttribute("Knockback Power");

        basicAttack = ability as Neutrophil_Grenade;

        grenades = GameObject.Find("Neutrophil Grenade Pool").GetComponentInChildren<ObjectPool>();
    }
}