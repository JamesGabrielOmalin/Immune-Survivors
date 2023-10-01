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
        float tick = 0f;

        var mobility = ability as Neutrophil_Mobility;
        //CharacterController controller = owner.GetComponentInParent<CharacterController>();
        Rigidbody rigidbody = owner.transform.root.GetComponent<Rigidbody>();
        var input = owner.transform.root.GetComponent<PlayerInput>().MoveInput;

        Vector3 direction = new(input.x, 0, input.y);

        if (direction == Vector3.zero)
        {
            yield break;
        }

        IsDashing = true;

        //Physics.IgnoreLayerCollision(6, 11, true);

        var velocity = direction *mobility.DashSpeed;
        var deltaPos = direction * (mobility.DashSpeed * Time.fixedDeltaTime);

        rigidbody.AddForce(direction * mobility.DashSpeed, ForceMode.VelocityChange);

        while (tick < mobility.MaxDashTime)
        {
            tick += Time.fixedDeltaTime;
            rigidbody.velocity = velocity;
            //rigidbody.AddForce(deltaPos, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();

            Debug.Log($"Dash: {rigidbody.velocity}");

            //var newPos = owner.transform.position;
            //newPos.y = y;
            //owner.transform.position = newPos;
        }
        //Physics.IgnoreLayerCollision(6, 11, false);

        CurrentCD = ability.Cooldown * (100f / 100f + CDReduction.Value);
        owner.StartCoroutine(UpdateCD());

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