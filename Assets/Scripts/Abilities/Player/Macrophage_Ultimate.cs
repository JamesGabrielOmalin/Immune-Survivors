using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "Macrophage_Ultimate", menuName = "Ability System/Abilities/Macrophage Ultimate")]
public class Macrophage_Ultimate : Ability
{
    [field: SerializeField] public float Duration { get; private set; }

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        return new Macrophage_UltimateSpec(this, owner);
    }
}

public class Macrophage_UltimateSpec : AbilitySpec
{
    public Macrophage_UltimateSpec(Macrophage_Ultimate ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
    }

    public AttributeSet attributes;
    public Attribute level;
    public Attribute attackDamage;
    public Attribute attackSpeed;
    public Attribute attackRange;
    public Attribute armor;
    public Attribute knockbackPower;
    public Attribute dotAmount;
    public Attribute dotDuration;
    public Attribute dotTickRate;

    private SpriteRenderer sprite;
    private Animator animator;

    // TODO: Make required level visible on ScriptableObject
    public override bool CanActivateAbility()
    {
        return level.Value >= 5f && base.CanActivateAbility();
    }

    public override IEnumerator ActivateAbility()
    {
        Macrophage_Ultimate ultimate = ability as Macrophage_Ultimate;
        
        var playable = owner.GetComponent<PlayableDirector>();
        playable.Play();

        animator.SetTrigger("Ultimate");

        foreach (Transform child in sprite.transform)
        {
            child.gameObject.SetActive(false);
        }

        // 100% increase in attack values
        AttributeModifier attackMod = new(1f, AttributeModifierType.Multiply);
        attackDamage.AddModifier(attackMod);
        attackSpeed.AddModifier(attackMod);
        attackRange.AddModifier(attackMod);

        AttributeModifier knockbackMod = new(5f, AttributeModifierType.Add);
        knockbackPower.AddModifier(knockbackMod);

        AttributeModifier armorMod = new((level.Value + 1) * 2f, AttributeModifierType.Add);
        armor.AddModifier(armorMod);

        // 300% increased DoT
        AttributeModifier dotMod = new(3f, AttributeModifierType.Multiply);
        dotAmount.AddModifier(dotMod);
        dotDuration.AddModifier(dotMod);
        
        AudioManager.instance.Play("MacrophageUltimate", owner.transform.position);
        yield return new WaitForSeconds(ultimate.Duration);

        foreach (Transform child in sprite.transform)
        {
            child.gameObject.SetActive(true);
        }

        // Remove all modifiers
        attackDamage.RemoveModifier(attackMod);
        attackSpeed.RemoveModifier(attackMod);
        attackRange.RemoveModifier(attackMod);

        knockbackPower.RemoveModifier(knockbackMod);

        dotAmount.RemoveModifier(dotMod);
        dotDuration.RemoveModifier(dotMod);

        armor.RemoveModifier(armorMod);

        CurrentCD = MaxCD;
        owner.StartCoroutine(UpdateCD());

        yield break;
    }

    public void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();

        level = attributes.GetAttribute("Level");
        attackDamage = attributes.GetAttribute("Attack Damage");
        attackSpeed = attributes.GetAttribute("Attack Speed");
        attackRange = attributes.GetAttribute("Attack Range");
        knockbackPower = attributes.GetAttribute("Knockback Power");
        armor = attributes.GetAttribute("Armor");
        dotAmount = attributes.GetAttribute("DoT Amount");
        dotDuration = attributes.GetAttribute("DoT Duration");
        dotTickRate = attributes.GetAttribute("DoT Tick Rate");
        CDReduction = attributes.GetAttribute("CD Reduction");

        sprite = owner.GetComponentInChildren<SpriteRenderer>();
        animator = sprite.GetComponent<Animator>();
    }
}