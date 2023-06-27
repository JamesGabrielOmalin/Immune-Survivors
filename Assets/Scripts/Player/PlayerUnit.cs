using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerUnitType
{
    Neutrophil,
    Macrophage,
    Dendritic
}

public class PlayerUnit : Unit
{
    [field: SerializeField] public AbilitySet AbilitySet { get; private set; }

    [field:SerializeField] public PlayerUnitType UnitType { get; private set; }

    private void Start()
    {
        StartCoroutine(Attack());
    }

    public void Upgrade()
    {
        Debug.Log("Upgrade");
    }

    private IEnumerator Attack()
    {
        yield return null;

        while (this)
        {
            yield return AbilitySet.BasicAttack.ActivateAbility();
        }
    }
}
