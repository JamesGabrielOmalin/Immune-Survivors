using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum NeutrophilAttackType
{
    Radial,
    Spread,
}

[CreateAssetMenu(fileName = "Neutrophil_BasicAttack", menuName = "Ability System/Abilities/Neutrophil Basic Attack")]
public class Neutrophil_BasicAttack : Ability
{
    public NeutrophilAttackType type = NeutrophilAttackType.Radial;

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Neutrophil_BasicAttackSpec(this, owner);
        return spec;
    }
}

public class Neutrophil_BasicAttackSpec : AbilitySpec
{
    private Neutrophil_BasicAttack basicAttack;
    private ObjectPool bullets;

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

    public Neutrophil_BasicAttackSpec(Neutrophil_BasicAttack ability, AbilitySystem owner) : base(ability, owner)
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

        // start shooting
        if (owner.GetComponent<AbilitySet>().CanUseBasicAttack)
            Shoot();

        yield break;
    }

    public override void EndAbility()
    {
        IsAttacking = false;
        base.EndAbility();
    }

    private void Shoot()
    {
        // implement basic shooting towards target
        GameObject target = EnemyManager.instance.GetNearestEnemy(owner.transform.position, attackRange.Value);
        if (target == null)
        {
            return;
        }

        SpawnProjectile(target);
    }

    private void SpawnProjectile(in GameObject target)
    {
        float angle = 0;
        float angleSteps = 0;
        float spreadFactor = 10;

        // angle threshold
        switch (basicAttack.type)
        {
            case NeutrophilAttackType.Spread:
                angle = -(attackCount.Value * spreadFactor)/2;
                angleSteps = spreadFactor;
                break;

            case NeutrophilAttackType.Radial:
                angle = 0;
                angleSteps = 360f / attackCount.Value;
                break;
        }

        Vector3 targetPos = target.transform.position;

        // Apply accuracy Offset
        Vector3 newTargetPos = targetPos;
        newTargetPos.y = owner.transform.position.y;

        int attackCountValue = (int)attackCount.Value;

        for (int i = 0; i < attackCountValue; i++)
        {
            GameObject bulletObject = bullets.RequestPoolable(owner.transform.position);
            if (bulletObject == null)
                continue;
            NeutrophilBullet bullet = bulletObject.GetComponent<NeutrophilBullet>();

            // Snapshot attributes
            //bulletObject.name = bulletObject.name + "(" + i.ToString() + ") ";
            bullet.attackDamage = attackDamage.Value;
            bullet.critRate = critRate.Value;
            bullet.critDMG = critDMG.Value;
            bullet.knockbackPower = knockbackPower.Value;
            bullet.transform.localScale = Vector3.one * attackSize.Value;

            // Calculate for the direction 
            Vector3 direction = (newTargetPos - owner.transform.position).normalized;

            // Apply towards the target
            bulletObject.transform.forward = direction;

            // Apply Angle offset
            //Vector3 forwardDir = projectile.transform.forward;
            //Quaternion offsetRotation = Quaternion.Euler(0f, angle, 0f);

            //Vector3 finalForwardDirection = offsetRotation * forwardDir;
            bulletObject.transform.Rotate(new Vector3(0f, angle, 0f));
            bulletObject.transform.localEulerAngles = new Vector3(0f, MathUtils.WrapAngle(bulletObject.transform.localEulerAngles.y), 0f);

            // increment angle based on nprojectiles
            angle += angleSteps;
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
        knockbackPower = attributes.GetAttribute("Knockback Power");

        basicAttack = ability as Neutrophil_BasicAttack;
        bullets = GameObject.Find("Neutrophil Bullet Pool").GetComponentInChildren<ObjectPool>();
    }
}