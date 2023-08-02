using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute
{
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
