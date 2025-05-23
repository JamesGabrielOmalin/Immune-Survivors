using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Dendritic_Mobility", menuName = "Ability System/Abilities/Dendritic Mobility")]
public class Dendritic_Mobility : Ability
{
    [field: SerializeField] public float DashDistance { get; private set; }

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Dendritic_MobilitySpec(this, owner);
        return spec;
    }
}

public class Dendritic_MobilitySpec : AbilitySpec
{
    public Dendritic_MobilitySpec(Dendritic_Mobility ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
    }

    private Dendritic_Mobility mobility;

    private AttributeSet attributes;
    private Attribute attackDamage;
    private Attribute attackSpeed;
    private Attribute critRate;
    private Attribute critDMG;
    public Attribute Type_1_DMG_Bonus;
    public Attribute Type_2_DMG_Bonus;
    public Attribute Type_3_DMG_Bonus;

    private readonly LayerMask layerMask = LayerMask.GetMask("Enemy");

    public bool IsDashing { get; private set; } = false;

    public override bool CanActivateAbility()
    {
        return base.CanActivateAbility() && !IsDashing;
    }

    private Collider collider;
    private PlayerMovement movement;
    private SpriteRenderer sprite;
    private Animator animator;
    private PlayerUnit unit;
    private const float radius = 1.25f;

    public override IEnumerator ActivateAbility()
    {
        animator.SetTrigger("Mobility");

        Vector3 direction = movement.lastInputDir;
        Vector3 startPos = owner.transform.position;
        Vector3 endPos = startPos + (direction * mobility.DashDistance);

        Vector3 rayDir = direction;
        float rayLength = mobility.DashDistance;

        collider.enabled = false;
        movement.enabled = false;

        Physics.IgnoreLayerCollision(6, 11, true);
        IsDashing = true;
        float AS = attackSpeed.Value;
        unit.StartIFrames();
        yield return new WaitForSeconds(Mathf.Lerp(0.25f, 0.1f, AS * 0.5f));

        Physics.IgnoreLayerCollision(6, 11, false);

        AudioManager.instance.Play("DentriticMovement", owner.transform.position);
        movement.transform.position = endPos;

        var hits = Physics.SphereCastAll(startPos, radius, rayDir, rayLength, layerMask);
        hits.Concat(Physics.SphereCastAll(endPos, radius * 2, rayDir, layerMask));

        float AD = attackDamage.Value;
        float CRIT_RATE = critRate.Value;
        float CRIT_DMG = critDMG.Value;

        float Type_1 = Type_1_DMG_Bonus.Value;
        float Type_2 = Type_2_DMG_Bonus.Value;
        float Type_3 = Type_3_DMG_Bonus.Value;

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out Enemy enemy))
            {
                float MaxHP = enemy.MaxHP.Value;
                float HP = enemy.HP.Value;

                float DMGBonus = Type_1_DMG_Bonus.Value;

                switch (enemy.Type)
                {
                    case AntigenType.Type_1:
                        DMGBonus = Type_1;
                        break;
                    case AntigenType.Type_2:
                        DMGBonus = Type_2;
                        break;
                    case AntigenType.Type_3:
                        DMGBonus = Type_3;
                        break;
                }

                float ratio = Mathf.SmoothStep(0.25f, 0.75f, (HP / MaxHP));
                float missingHPBonusDMG = Mathf.Lerp(1.1f, 0.1f, ratio);

                //enemy.TakeDamage(damage);
                float damage = AD * DMGBonus * missingHPBonusDMG;
                float armor = enemy.Armor.Value;
                DamageCalculator.ApplyDamage(damage, CRIT_RATE, CRIT_DMG, armor, enemy);
                enemy.GetComponent<ImpactReceiver>().AddImpact(rayDir, rayLength);
            }
        }

        bool resetCD = false;

        // Check for dead enemies
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                // If at least 1 enemy was killed, reset cooldown
                if (enemy.IsDead)
                {
                    // Get bonus Antigen
                    AntigenManager.instance.AddBonusAntigen(enemy.Type);
                    AudioManager.instance.Play("PlayerPickUp", owner.transform.position);
                    resetCD = true;
                    break;
                }
            }
        }

        // Teleport behind the last enemy hit
        //if (hits.Length > 0)
        //{
        //    if (Vector3.Distance(startPos, hits.Last().point) > Vector3.Distance(startPos, endPos))
        //        owner.transform.position = hits.Last().point + (rayDir * 1.5f);
        //}

        // If no enemy was killed, do not reset CD
        if (!resetCD)
        {
            CurrentCD = MaxCD;
            owner.StartCoroutine(UpdateCD());
        }

        owner.transform.localPosition = Vector3.zero;
        yield break;
    }

    public override void EndAbility()
    {
        base.EndAbility();
        IsDashing = false;

        PlayerMovement movement = owner.GetComponentInParent<PlayerMovement>();
        Collider collider = owner.GetComponent<Collider>();
        collider.enabled = true;
        movement.enabled = true;
    }

    private void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();
        attackDamage = attributes.GetAttribute(Attribute.ATTACK_DAMAGE);
        attackSpeed = attributes.GetAttribute(Attribute.ATTACK_SPEED);
        critRate = attributes.GetAttribute(Attribute.CRITICAL_RATE);
        critDMG = attributes.GetAttribute(Attribute.CRITICAL_DAMAGE);
        CDReduction = attributes.GetAttribute(Attribute.CD_REDUCTION);

        Type_1_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_1_DMG_BONUS);
        Type_2_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_2_DMG_BONUS);
        Type_3_DMG_Bonus = attributes.GetAttribute(Attribute.TYPE_3_DMG_BONUS);

        mobility = ability as Dendritic_Mobility;

        collider = owner.GetComponent<Collider>(); 
        movement = owner.GetComponentInParent<PlayerMovement>();
        sprite = owner.GetComponentInChildren<SpriteRenderer>();
        animator = sprite.GetComponent<Animator>();
        unit = owner.GetComponent<PlayerUnit>();
    }
}