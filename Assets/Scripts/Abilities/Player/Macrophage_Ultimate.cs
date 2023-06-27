using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Attribute numProjectiles;
    public Attribute attackDamage;
    public Attribute criticalChance;
    public Attribute attackSpeed;
    public Attribute attackRange;
    public Attribute knockbackPower;
    public Attribute pincerSize;
    public Attribute pincerDelay;
    public Attribute accuracy;
    public Attribute damageDelay;

    // TODO: Make required level visible on ScriptableObject
    public override bool CanActivateAbility()
    {
        return level.Value >= 6f && base.CanActivateAbility();
    }

    public override IEnumerator ActivateAbility()
    {
        Macrophage_Ultimate ultimate = ability as Macrophage_Ultimate;

        // 50% increase in attack values
        AttributeModifier attackMod = new(0.5f, AttributeModifierType.Multiply);
        attackDamage.AddModifier(attackMod);
        attackSpeed.AddModifier(attackMod);
        attackRange.AddModifier(attackMod);

        AttributeModifier critMod = new(0.25f, AttributeModifierType.Add);
        criticalChance.AddModifier(critMod);

        // 100% increase in size
        AttributeModifier sizeMod = new(1f, AttributeModifierType.Multiply);
        pincerSize.AddModifier(sizeMod);

        owner.transform.localScale = new(2, 2, 2);

        yield return new WaitForSeconds(ultimate.Duration);

        owner.transform.localScale = new(1, 1, 1);

        // Remove all modifiers
        attackDamage.RemoveModifier(attackMod);
        attackSpeed.RemoveModifier(attackMod);
        attackRange.RemoveModifier(attackMod);
        criticalChance.RemoveModifier(critMod);
        pincerSize.RemoveModifier(sizeMod);

        CurrentCD = ability.Cooldown;
        owner.StartCoroutine(UpdateCD());

        yield break;
    }

    public void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();

        level = attributes.GetAttribute("Level");
        numProjectiles = attributes.GetAttribute("Num Projectiles");
        attackDamage = attributes.GetAttribute("Attack Damage");
        criticalChance = attributes.GetAttribute("Critical Chance");
        attackSpeed = attributes.GetAttribute("Attack Speed");
        attackRange = attributes.GetAttribute("Attack Range");
        knockbackPower = attributes.GetAttribute("Knockback Power");
        pincerSize = attributes.GetAttribute("Pincer Size");
        pincerDelay = attributes.GetAttribute("Pincer Delay");
        accuracy = attributes.GetAttribute("Accuracy");
        damageDelay = attributes.GetAttribute("Damage Delay");
    }
}