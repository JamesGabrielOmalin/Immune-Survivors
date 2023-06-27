using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveCell : MonoBehaviour
{
    [HideInInspector] public AntigenType Type { get; private set; }
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private List<Color> colors = new();

    public void SetType(AntigenType newType)
    {
        Type = newType;

        sprite.color = colors[(int)newType];
    }
}
