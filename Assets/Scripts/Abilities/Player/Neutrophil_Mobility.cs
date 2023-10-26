using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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

    public bool IsDashing { get; private set; } = false;

    private SpriteRenderer sprite;
    private Animator animator;

    private VisualEffect vfx;

    public override bool CanActivateAbility()
    {
        return base.CanActivateAbility() && !IsDashing && rigidbody.velocity.sqrMagnitude > 0f;
    }

    private Rigidbody rigidbody;
    private PlayerInput playerInput;
    private readonly WaitForFixedUpdate wait = new();

    public override IEnumerator ActivateAbility()
    {
        float tick = 0f;

        var mobility = ability as Neutrophil_Mobility;
        //CharacterController controller = owner.GetComponentInParent<CharacterController>();
        
        var input = playerInput.MoveInput;

        Vector3 direction = new(input.x, 0, input.y);

        animator.SetTrigger("Mobility");
        vfx.SetVector3("Direction", direction);
        vfx.Play();

        IsDashing = true;

        Physics.IgnoreLayerCollision(6, 11, true);

        var velocity = direction *mobility.DashSpeed;

        AudioManager.instance.Play("NeutrophilMovement", owner.transform.position);
        rigidbody.AddForce(direction * mobility.DashSpeed, ForceMode.VelocityChange);

        while (tick < mobility.MaxDashTime)
        {
            tick += Time.fixedDeltaTime;
            rigidbody.velocity = velocity;
            //rigidbody.AddForce(deltaPos, ForceMode.VelocityChange);
            yield return wait;

            //Debug.Log($"Dash: {rigidbody.velocity}");

            //var newPos = owner.transform.position;
            //newPos.y = y;
            //owner.transform.position = newPos;
        }
        Physics.IgnoreLayerCollision(6, 11, false);
        rigidbody.velocity = Vector3.zero;
        vfx.Stop();

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

        sprite = owner.GetComponentInChildren<SpriteRenderer>();
        animator = sprite.GetComponent<Animator>();
        rigidbody = owner.transform.root.GetComponent<Rigidbody>();
        playerInput = owner.transform.root.GetComponent<PlayerInput>();

        vfx = owner.transform.Find("Neutrophil Dash VFX").GetComponent<VisualEffect>();
    }
}