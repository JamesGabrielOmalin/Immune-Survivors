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
                float duration = AntigenManager.instance.GetAntigenCount(Type) * 0.01f;
                enemy.ApplyStun(duration);
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
