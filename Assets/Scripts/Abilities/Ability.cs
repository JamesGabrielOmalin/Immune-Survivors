using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    BasicAttack,
    Ability,
    Ultimate
}

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

    [field: SerializeField] public AbilityType AbilityType { get; private set; }

    [field: SerializeField] public bool HasCharges { get; private set; }

    [field: SerializeField] public int MaxCharges { get; private set; }

    public abstract AbilitySpec CreateSpec(AbilitySystem owner);
}

public abstract class AbilitySpec
{
    public AbilitySpec(Ability ability, AbilitySystem owner)
    {
        this.ability = ability;
        this.owner = owner;
        this.abilityLevel = 1;
        this.CurrentCharge = ability.MaxCharges;
    }

    public Ability ability;
    protected AbilitySystem owner;
    public int abilityLevel;

    public bool IsActive { get; private set; } = false;

    public System.Action OnAbilityActivateFailed;
    public System.Action OnAbilityCooldownStart;
    public System.Action OnAbilityCooldownEnd;
    public System.Action OnChargeGained;
    public System.Action OnChargeLost;

    public bool IsCharged => CurrentCharge > 0;

    protected Attribute CDReduction;
    public float MaxCD => ability.Cooldown * (100f / (100f + CDReduction.Value));

    public virtual bool CanActivateAbility()
    {
        if (ability.HasCharges)
            return IsCharged && !IsActive;

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
        if (ability.HasCharges)
        {
            CurrentCharge--;
            OnChargeLost?.Invoke();
            Debug.Log($"Charges: {CurrentCharge}");
        }
        yield return ActivateAbility();
        EndAbility();
    }

    public abstract IEnumerator ActivateAbility();

    public virtual void EndAbility()
    {
        IsActive = false;
    }

    public float CurrentCD { get; protected set; }
    public int CurrentCharge { get; protected set; }

    private IEnumerator ChargeCDEnumerator = null;

    protected IEnumerator UpdateCD()
    {
        if (ability.HasCharges)
        {
            if (ChargeCDEnumerator != null)
                yield break;
            ChargeCDEnumerator = UpdateCD_WithCharge();
            yield return ChargeCDEnumerator;
            ChargeCDEnumerator = null;
            yield break;
        }

        OnAbilityCooldownStart?.Invoke();
        while (CurrentCD > 0f)
        {
            CurrentCD -= Time.deltaTime;
            yield return null;
        }

        CurrentCD = 0f;
        OnAbilityCooldownEnd?.Invoke();
    }

    protected IEnumerator UpdateCD_WithCharge()
    {
        OnAbilityCooldownStart?.Invoke();

        while (CurrentCharge < ability.MaxCharges)
        {
            CurrentCD = MaxCD;
            while (CurrentCD > 0f)
            {
                CurrentCD -= Time.deltaTime;
                yield return null;
            }

            CurrentCD = 0f;
            CurrentCharge++;
            OnChargeGained?.Invoke();
            yield return null;
        }

        OnAbilityCooldownEnd?.Invoke();
    }
}