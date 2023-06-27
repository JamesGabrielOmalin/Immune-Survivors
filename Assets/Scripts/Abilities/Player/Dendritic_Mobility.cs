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

    }

    public bool IsDashing { get; private set; } = false;

    public override bool CanActivateAbility()
    {
        return base.CanActivateAbility() && !IsDashing;
    }

    public override IEnumerator ActivateAbility()
    {
        Attribute AttackDamage = owner.GetComponent<AttributeSet>().GetAttribute("Attack Damage");

        var mobility = ability as Dendritic_Mobility;

        PlayerMovement movement = owner.GetComponent<PlayerMovement>();
        CharacterController controller = owner.GetComponent<CharacterController>();
        BodyCollider bodyCollider = owner.GetComponent<BodyCollider>();

        Vector3 direction = movement.inputDir;
        Vector3 startPos = owner.transform.position;
        Vector3 endPos = startPos + (direction * mobility.DashDistance);

        Debug.Log($"Start: {startPos}, End: {endPos}, Dir: {direction}");

        Vector3 rayDir = direction;
        float rayLength = mobility.DashDistance;

        controller.detectCollisions = false;
        controller.enabled = false;
        bodyCollider.enabled = false;

        IsDashing = true;
        yield return new WaitForSeconds(0.25f);

        owner.transform.position = endPos;

        var hits = Physics.SphereCastAll(startPos, controller.radius, rayDir, rayLength, LayerMask.GetMask("Enemy"));

        // Deal damage equal to 150% AD scaling
        float Damage = AttackDamage.Value * 1.5f;
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(Damage);
                enemy.GetComponent<ImpactReceiver>().AddImpact(rayDir, rayLength * 2f);
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
                    resetCD = true;
                    break;
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
            CurrentCD = ability.Cooldown;
            owner.StartCoroutine(UpdateCD());
        }

        yield break;
    }

    public override void EndAbility()
    {
        base.EndAbility();
        IsDashing = false;

        CharacterController controller = owner.GetComponent<CharacterController>();
        BodyCollider bodyCollider = owner.GetComponent<BodyCollider>();

        controller.enabled = true;
        controller.detectCollisions = true;
        bodyCollider.enabled = true;
    }

    private void ResetCD()
    {
        CurrentCD = 0f;
    }
}