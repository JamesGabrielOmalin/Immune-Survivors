using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutrophilSlowField : MonoBehaviour, IBodyColliderListener
{
    [HideInInspector] public float slowAmount;

    private AttributeModifier slow;

    private readonly List<Enemy> enemies = new();

    private void OnEnable()
    {
        slow = new(slowAmount, AttributeModifierType.Multiply);
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
    }
}
