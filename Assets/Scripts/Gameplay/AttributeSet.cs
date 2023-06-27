using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeSet : MonoBehaviour
{
    [SerializeField]
    private List<Attribute> Attributes = new();

    // Finds the attribute with the same name
    public Attribute GetAttribute(string name) => Attributes.Find(attribute => attribute.Name == name);
    public List<Attribute> GetAttributeList() => Attributes;
}
