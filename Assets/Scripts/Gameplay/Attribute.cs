using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute
{
    public const string LEVEL = "Level";
    public const string MAX_HP = "Max HP";
    public const string HP = "HP";
    public const string HP_REGEN = "HP Regen";

    public const string ATTACK_DAMAGE = "Attack Damage";
    public const string ATTACK_SPEED = "Attack Speed";
    public const string ATTACK_RANGE = "Attack Range";
    public const string ATTACK_COUNT = "Attack Count";
    public const string ATTACK_SIZE = "Attack Size";

    public const string CRITICAL_RATE = "Critical Rate";
    public const string CRITICAL_DAMAGE = "Critical Damage";

    public const string KNOCKBACK_POWER = "Knockback Power";
    public const string CD_REDUCTION = "CD Reduction";

    public const string MOVE_SPEED = "Move Speed";
    public const string DASH_RANGE = "Dash Range";

    public const string ARMOR = "Armor";

    public const string TYPE_1_DMG_BONUS = "Type_1 DMG Bonus";
    public const string TYPE_2_DMG_BONUS = "Type_2 DMG Bonus";
    public const string TYPE_3_DMG_BONUS = "Type_3 DMG Bonus";

    public const string DOT_AMOUNT = "DoT Amount";
    public const string DOT_DURATION = "DoT Duration";
    public const string DOT_TICK_RATE = "DoT Tick Rate";

    public Attribute(string name, float baseValue)
    {
        this.Name = name;
        this.BaseValue = baseValue;
    }

    [field: SerializeField]
    public string Name { get; private set; } = string.Empty;
    [SerializeField]
    private float baseValue; 
    public float BaseValue
    {
        get => baseValue;
        set
        {
            float OldValue = Value;
            baseValue = value;
            float NewValue = Value;

            OnAttributeModified?.Invoke(OldValue, NewValue);
        }
    }

    [field: SerializeField]
    public List<AttributeModifier> Modifiers { get; private set; } = new();

    private bool isClean;

    private float value;

    // Gets the value of the Attribute, along with its modifiers
    public float Value
    {
        get
        {
            if (!isClean)
            {
                value = CalculateFinalValue();
                isClean = true;
            }

            return value;
        }
    }

    private float CalculateFinalValue()
    {
        var Override = Modifiers.FindLast(mod => mod.Type == AttributeModifierType.Override);

        if (Override != null)
        {
            return Override.Value;
        }

        float Add = 0f, Multiply = 1f;

        foreach (var Modifier in Modifiers)
        {
            switch (Modifier.Type)
            {
                case AttributeModifierType.Add:
                    Add += Modifier.Value;
                    break;
                case AttributeModifierType.Multiply:
                    Multiply += Modifier.Value;
                    break;
            }
        }
        return (BaseValue + Add) * Multiply;
    }

    public System.Action<float, float> OnAttributeModified;

    public void AddModifier(AttributeModifier Modifier)
    {
        float OldValue = Value;
        Modifiers.Add(Modifier);
        float NewValue = Value;

        isClean = false;
        OnAttributeModified?.Invoke(OldValue, NewValue);
    }
    public void RemoveModifier(AttributeModifier Modifier)
    {
        float OldValue = Value;
        Modifiers.Remove(Modifier);
        float NewValue = Value;

        isClean = false;
        OnAttributeModified?.Invoke(OldValue, NewValue);
    }

    public void RemoveAllModifiers()
    {
        float OldValue = Value;
        Modifiers.Clear();
        float NewValue = Value;

        isClean = false;
        OnAttributeModified?.Invoke(OldValue, NewValue);
    }

    /// <summary>
    /// Directly modifies the BaseValue
    /// </summary>
    public void ApplyInstantModifier(AttributeModifier Modifier)
    {
        float OldValue = Value;

        switch (Modifier.Type)
        {
            case AttributeModifierType.Add:
                BaseValue += Modifier.Value;
                break;
            case AttributeModifierType.Multiply:
                BaseValue *= Modifier.Value;
                break;
        }

        float NewValue = Value;

        isClean = false;
        OnAttributeModified?.Invoke(OldValue, NewValue);
    }
}
