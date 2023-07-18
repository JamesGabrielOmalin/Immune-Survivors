using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutrophilSlowField : MonoBehaviour, IBodyColliderListener
{
    [HideInInspector] public float slowAmount;

    private AttributeModifier slow;

    private readonly List<Enemy> enemies = new();

    private void Start()
    {
        slow = new(slowAmount, AttributeModifierType.Multiply);
    }

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.attributes.GetAttribute("Move Speed").AddModifier(slow);
            enemies.Add(enemy);
            Debug.Log("Enemy slowed");
        }
    }

    public void OnBodyColliderExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.attributes.GetAttribute("Move Speed").RemoveModifier(slow);
            enemies.Remove(enemy);
            Debug.Log("Enemy unslowed");
        }
    }

    private void OnDisable()
    {
        foreach (var enemy in enemies)
        {
            enemy.attributes.GetAttribute("Move Speed").RemoveModifier(slow);
            Debug.Log("Disabled field: Enemy unslowed");
        }


    }
}
