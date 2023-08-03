using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antibody : Projectile, IBodyColliderListener
{
    [HideInInspector] public AntigenType Type { get; private set; }
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private List<Color> colors = new();

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if (enemy.Type == this.Type)
            {
                // 1% Armor Shred per 10 Antigen
                float amount = (AntigenManager.instance.GetAntigenCount(Type) / 10f) * 0.01f;
                enemy.ApplyArmorShred(amount);
                this.gameObject.SetActive(false);
            }
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
