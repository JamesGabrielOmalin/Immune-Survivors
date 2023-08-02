using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cytokine : Projectile, IBodyColliderListener
{
    [HideInInspector] public AntigenType Type { get; private set; }
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private List<Color> colors = new();

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            AttributeModifier mod = new(AntigenManager.instance.GetAntigenCount(Type) * 0.01f, AttributeModifierType.Multiply);
            player.ApplyAntigenBuffs(Type, mod, 2.5f);
            this.gameObject.SetActive(false);
            Debug.Log("Apply buff");
        }
    }

    public void OnBodyColliderExit(Collider other)
    {

    }

    public void SetType(AntigenType newType)
    {
        Type = newType;

        sprite.color = colors[(int)newType];
    }
}
