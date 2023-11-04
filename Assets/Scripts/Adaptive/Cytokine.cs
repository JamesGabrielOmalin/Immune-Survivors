using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Cytokine : MonoBehaviour, IBodyColliderListener
{
    [HideInInspector] public AntigenType Type { get; private set; }
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private List<Color> colors = new();

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            int antigenCount = AntigenManager.instance.GetAntigenCount(Type);

            // 0.05% DMG Bonus per antigen against specific enemy
            AttributeModifier mod = new(antigenCount * 0.005f, AttributeModifierType.Add);
            float duration = 3f + antigenCount * 0.001f;
            player.ApplyAntigenBuffs(Type, mod, duration);
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

        var color = colors[(int)newType];

        sprite.color = color;
        vfx.SetVector4("Color", color);
    }
} 
