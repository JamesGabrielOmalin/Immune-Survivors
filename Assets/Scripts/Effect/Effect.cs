using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    Buff,
    Weapon,
    Lifestyle,
}

public enum EffectDurationType
{
    Infinite,
    Duration
}

[System.Serializable]
public class EffectModifier
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public AttributeModifier Modifier { get; private set; }
}

[CreateAssetMenu(fileName = "Effect", menuName = "Ability System/Effect")]
public class Effect : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public List<string> Descriptions { get; private set; } = new();
    [field: SerializeField] public EffectType EffectType { get; private set; }
    [field: SerializeField] public List<EffectModifier> Modifiers { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }

    [field: Header("Duration")]
    [field: SerializeField] public EffectDurationType DurationType { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }

    [field: Header("Ability")]
    [field: SerializeField] public List<Ability> Abilities { get; private set; }

    public EffectSpec CreateSpec(AbilitySystem source, AbilitySystem target)
    {
        return new EffectSpec(this, source, target);
    }
}

public class EffectSpec
{
    public EffectSpec(Effect effect, AbilitySystem source, AbilitySystem target)
    {
        this.effect = effect;
        CurrentDuration = effect.Duration;
        this.source = source;
        this.target = target;

        Apply();
    }

    public Effect effect;
    public AbilitySystem source;
    public AbilitySystem target;
    public float CurrentDuration { get; protected set; }

    private readonly List<EffectModifier> appliedModifiers = new();
    private readonly List<AbilitySpec> grantedAbilities = new();

    public virtual void Update()
    {
        if (effect.DurationType != EffectDurationType.Duration)
            return;

        CurrentDuration -= Time.deltaTime;

        if (CurrentDuration <= 0f)
            Expire();
    }

    private void Apply()
    {
        AttributeSet attributes = target.GetComponent<AttributeSet>();

        foreach (var mod in effect.Modifiers)
        {
            Attribute attribute = attributes.GetAttribute(mod.Name);

            if (attribute == null)
                continue;

            attribute.AddModifier(mod.Modifier);
            appliedModifiers.Add(mod);
        }

        foreach (var ability in effect.Abilities)
        {
            if (!target.HasAbility(ability))
                target.GrantAbility(ability.CreateSpec(source));
            else
            {
                target.GetAbility(ability).abilityLevel++;
                Debug.Log("UPGRADE!");
            }
        }
    }

    private void Remove()
    {
        AttributeSet attributes = target.GetComponent<AttributeSet>();

        foreach (var mod in appliedModifiers)
        {
            Attribute attribute = attributes.GetAttribute(mod.Name);

            if (attribute == null)
                continue;

            attribute.RemoveModifier(mod.Modifier);
        }

        appliedModifiers.Clear();

        foreach (var ability in grantedAbilities)
        {
            target.RemoveAbility(ability);
        }

        grantedAbilities.Clear();
    }

    public virtual void Expire()
    {
        Remove();
    }
}