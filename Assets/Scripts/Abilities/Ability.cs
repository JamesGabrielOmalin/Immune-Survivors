using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{  
    /// <summary>
    /// The name of this ability
    /// </summary>
    [field: SerializeField] public string Name { get; private set; }

    /// <summary>
    /// The cooldown associated with this ability
    /// </summary>
    [field: SerializeField] public float Cooldown { get; private set; }

    public abstract AbilitySpec CreateSpec(AbilitySystem owner);
}

public abstract class AbilitySpec
{
    public AbilitySpec(Ability ability, AbilitySystem owner)
    {
        this.ability = ability;
        this.owner = owner;
    }

    protected Ability ability;
    protected AbilitySystem owner;

    public bool IsActive { get; private set; } = false;

    public System.Action OnAbilityActivateFailed;

    public virtual bool CanActivateAbility()
    {
        return !IsActive && CurrentCD <= 0f;
    }

    public virtual IEnumerator TryActivateAbility()
    {
        if (!CanActivateAbility())
        {
            OnAbilityActivateFailed?.Invoke();
            yield break;
        }

        IsActive = true;
        yield return ActivateAbility();
        EndAbility();
    }

    public abstract IEnumerator ActivateAbility();

    public virtual void EndAbility()
    {
        IsActive = false;
    }

    public float CurrentCD { get; protected set; }

    protected IEnumerator UpdateCD()
    {
        while (CurrentCD > 0f)
        {
            CurrentCD -= Time.deltaTime;
            yield return null;
        }

        CurrentCD = 0f;
    }
}