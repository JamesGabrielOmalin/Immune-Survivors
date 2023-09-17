using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class NeutrophilSlowField : MonoBehaviour, IBodyColliderListener
{
    [SerializeField] private VisualEffect vfx;
    [HideInInspector] public float slowAmount;
    [HideInInspector] public float duration;

    private AttributeModifier slow;

    private readonly List<Enemy> enemies = new();

    private void OnEnable()
    {
        slow = new(slowAmount, AttributeModifierType.Multiply);
        vfx.SetFloat("Duration", duration);
    }

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.attributes.GetAttribute("Move Speed").AddModifier(slow);
            enemies.Add(enemy);
        }
    }

    public void OnBodyColliderExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.attributes.GetAttribute("Move Speed").RemoveModifier(slow);
            enemies.Remove(enemy);
        }
    }

    private void OnDisable()
    {
        foreach (var enemy in enemies)
        {
            enemy.attributes.GetAttribute("Move Speed").RemoveModifier(slow);
        }

        enemies.Clear();
    }
}
