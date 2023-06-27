using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Macrophage_Mobility", menuName = "Ability System/Abilities/Macrophage Mobility")]
public class Macrophage_Mobility : Ability
{
    [field: SerializeField] public float Duration { get; private set; }

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Macrophage_MobilitySpec(this, owner);
        return spec;
    }
}

public class Macrophage_MobilitySpec : AbilitySpec
{
    public Macrophage_MobilitySpec(Macrophage_Mobility ability, AbilitySystem owner) : base(ability, owner)
    {

    }

    public override IEnumerator ActivateAbility()
    {
        var mobility = ability as Macrophage_Mobility;

        WaitForSeconds wait = new(mobility.Duration);

        CharacterController controller = owner.GetComponent<CharacterController>();
        BodyCollider bodyCollider = owner.GetComponent<BodyCollider>();

        controller.detectCollisions = false;
        bodyCollider.enabled = false;

        owner.gameObject.layer = 3;

        SpriteRenderer sprite = owner.GetComponentInChildren<SpriteRenderer>();
        sprite.material.renderQueue = 3000;
        sprite.color = new (1, 1, 1, 0.5f);

        yield return wait;

        sprite.material.renderQueue = 2450;
        sprite.color = Color.white;

        owner.gameObject.layer = 6;

        CurrentCD = ability.Cooldown;
        owner.StartCoroutine(UpdateCD());
    }

    public override void EndAbility()
    {
        base.EndAbility();

        CharacterController controller = owner.GetComponent<CharacterController>();
        BodyCollider bodyCollider = owner.GetComponent<BodyCollider>();

        controller.detectCollisions = true;
        bodyCollider.enabled = true;
    }
}