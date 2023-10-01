using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Neutrophil_Mobility", menuName = "Ability System/Abilities/Neutrophil Mobility")]
public class Neutrophil_Mobility : Ability
{
    [field: SerializeField] public float DashSpeed { get; private set; }
    [field: SerializeField] public float MaxDashTime { get; private set; }

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Neutrophil_MobilitySpec(this, owner);
        return spec;
    }
}

public class Neutrophil_MobilitySpec : AbilitySpec
{
    public Neutrophil_MobilitySpec(Neutrophil_Mobility ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
    }

    private AttributeSet attributes;
    private Attribute moveSpeed;
    private Attribute CDReduction;

    public bool IsDashing { get; private set; } = false;

    public override bool CanActivateAbility()
    {
        return base.CanActivateAbility() && !IsDashing;
    }

    public override IEnumerator ActivateAbility()
    {
        CurrentCD = ability.Cooldown * (100f / 100f + CDReduction.Value);
        owner.StartCoroutine(UpdateCD());

        float tick = 0f;

        var mobility = ability as Neutrophil_Mobility;
        CharacterController controller = owner.GetComponentInParent<CharacterController>();

        Vector3 direction = controller.velocity.normalized;

        if (controller.velocity.sqrMagnitude <= 0f)
            direction = Vector3.right;

        IsDashing = true;

        float y = owner.transform.position.y;
        while (tick < mobility.MaxDashTime)
        {
            tick += Time.deltaTime;
            controller.Move(direction * (moveSpeed.Value * mobility.DashSpeed * Time.deltaTime));

            yield return null;

            var newPos = owner.transform.position;
            newPos.y = y;
            owner.transform.position = newPos;
        }

        yield break;
    }

    public override void EndAbility()
    {
        IsDashing = false;
        base.EndAbility();
    }

    private void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();
        moveSpeed = attributes.GetAttribute("Move Speed");
        CDReduction = attributes.GetAttribute("CD Reduction");
    }
}