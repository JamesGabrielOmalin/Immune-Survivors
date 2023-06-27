using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeModifierType
{
    Add,
    Multiply,
    Override
}

[System.Serializable]
public class AttributeModifier
{
    public AttributeModifier(float value, AttributeModifierType type)
    {
        Value = value;
        Type = type;
    }

    [field: SerializeField]
    public float Value { get; private set; } = 0f;
    [field: SerializeField]
    public AttributeModifierType Type { get; private set; } = AttributeModifierType.Add;
}
