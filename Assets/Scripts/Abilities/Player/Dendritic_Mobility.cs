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
    private Attribute critDMG;
    private Attribute CDReduction;

    public bool IsDashing { get; private set; } = false;

    public override bool CanActivateAbility()
    {
        return base.CanActivateAbility() && !IsDashing;
    }

    public override IEnumerator ActivateAbility()
    {
        PlayerMovement movement = owner.GetComponentInParent<PlayerMovement>();
        Collider collider = owner.GetComponent<Collider>();

        Vector3 direction = movement.lastInputDir;
        Vector3 startPos = owner.transform.position;
        Vector3 endPos = startPos + (direction * mobility.DashDistance);

        Vector3 rayDir = direction;
        float rayLength = mobility.DashDistance;

        collider.enabled = false;
        movement.enabled = false;

        Physics.IgnoreLayerCollision(6, 11, true);
        IsDashing = true;
        yield return new WaitForSeconds(0.25f);

        Physics.IgnoreLayerCollision(6, 11, false);

        AudioManager.instance.Play("DentriticMovement", owner.transform.position);
        movement.transform.position = endPos;

        var hits = Physics.SphereCastAll(startPos, 1f, rayDir, rayLength, LayerMask.GetMask("Enemy"));

        // Guaranteed CRIT
        //float damage = DamageCalculator.CalcDamage(attackDamage.Value, 1f, critDMG.Value);

        float AD = attackDamage.Value;
        float CRIT_RATE = 1f;
        float CRIT_DMG = critDMG.Value;

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                //enemy.TakeDamage(damage);
                float armor = enemy.attributes.GetAttribute("Armor").Value;
                DamageCalculator.ApplyDamage(AD, CRIT_RATE, CRIT_DMG, armor, enemy);
                enemy.GetComponent<ImpactReceiver>().AddImpact(rayDir, rayLength);
            }
        }

        bool resetCD = false;
        // Check for dead enemies
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                // Get bonus Antigen
                AntigenManager.instance.AddAntigen(enemy.Type);

                // If at least 1 enemy was killed, reset cooldown
                if (!resetCD && enemy.IsDead)
                {
                    resetCD = true;
                }
            }
        }

        // Teleport behind the last enemy hit
        if (hits.Length > 0)
        {
            if (Vector3.Distance(startPos, hits.Last().point) > Vector3.Distance(startPos, endPos))
                owner.transform.position = hits.Last().point + (rayDir * 1.5f);
        }

        // If no enemy was killed, do not reset CD
        if (!resetCD)
        {
            CurrentCD = ability.Cooldown * (100f / 100f + CDReduction.Value);
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
        attackDamage = attributes.GetAttribute("Attack Damage");
        critDMG = attributes.GetAttribute("Critical Damage");
        CDReduction = attributes.GetAttribute("CD Reduction");

        mobility = ability as Dendritic_Mobility;
    }
}