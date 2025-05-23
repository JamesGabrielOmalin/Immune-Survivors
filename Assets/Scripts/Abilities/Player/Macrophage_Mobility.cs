using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Macrophage_Mobility", menuName = "Ability System/Abilities/Macrophage Mobility")]
public class Macrophage_Mobility : Ability
{
    [field: SerializeField] public float MoveSpeedBonus { get; private set; }
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
        Init();
    }

    private Macrophage_Mobility mobility;

    private AttributeSet attributes;
    private Attribute moveSpeed;
    private Attribute armor;
    private GameObject outline;

    public override IEnumerator ActivateAbility()
    {
        CurrentCD = MaxCD;
        WaitForSeconds wait = new(mobility.Duration);

        Physics.IgnoreLayerCollision(6, 11, true);

        SpriteRenderer sprite = owner.GetComponentInChildren<SpriteRenderer>();
        //sprite.material.renderQueue = 3000;
        sprite.color = new (0.75f, 0.375f, 0f, 0.5f); outline.SetActive(false);

        AttributeModifier mod = new(mobility.MoveSpeedBonus, AttributeModifierType.Multiply);
        moveSpeed.AddModifier(mod);
        AudioManager.instance.Play("MacrophageMovement", owner.transform.position);

        AttributeModifier armorMod = new(0.1f, AttributeModifierType.Multiply);
        armor.AddModifier(armorMod);

        yield return wait;

        moveSpeed.RemoveModifier(mod);
        armor.RemoveModifier(armorMod);

        //sprite.material.renderQueue = 2450;
        sprite.color = Color.white; outline.SetActive(true);

        owner.StartCoroutine(UpdateCD());
    }

    public override void EndAbility()
    {
        base.EndAbility();
        Physics.IgnoreLayerCollision(6, 11, false);
    }

    private void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();
        moveSpeed = attributes.GetAttribute(Attribute.MOVE_SPEED);
        armor = attributes.GetAttribute(Attribute.ARMOR);
        CDReduction = attributes.GetAttribute(Attribute.CD_REDUCTION);

        mobility = ability as Macrophage_Mobility;
        outline = owner.transform.Find("Sprite").Find("Outline").gameObject;
    }
}