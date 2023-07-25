using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem : MonoBehaviour
{
    public List<AbilitySpec> GrantedAbilities = new();
    public List<EffectSpec> GrantedEffects = new();
    public List<EffectSpec> GrantedEffectsWithDuration = new();

    public AbilitySpec GetAbility(Ability ability)
    {
        return GrantedAbilities.Find((spec) => spec.ability == ability);
    }

    public bool HasAbility(Ability ability)
    {
        return GrantedAbilities.Find((spec) => spec.ability == ability) != null;
    }

    public List<AbilitySpec> GetAbilitiesOfType(AbilityType type)
    {
        return GrantedAbilities.FindAll((abilitySpec) => abilitySpec.ability.AbilityType == type);
    }

    public void GrantAbility(AbilitySpec spec)
    {
        GrantedAbilities.Add(spec);
    }

    public void RemoveAbility(AbilitySpec spec)
    {
        GrantedAbilities.Remove(spec);
    }

    public void ApplyEffectToSelf(Effect effect)
    {
        var spec = effect.CreateSpec(this, this);
        this.AddEffectSpec(spec);
    }

    public void ApplyEffectToTarget(Effect effect, AbilitySystem target)
    {
        var spec = effect.CreateSpec(this, target);
        target.AddEffectSpec(spec);
    }

    private void AddEffectSpec(EffectSpec spec)
    {
        GrantedEffects.Add(spec);

        if (spec.effect.DurationType == EffectDurationType.Duration)
            GrantedEffectsWithDuration.Add(spec);
    }

    public void RemoveEffect(Effect effect)
    {
        var grantedEffect = GrantedEffects.Find(spec => spec.effect == effect);
        grantedEffect.Expire();

        GrantedEffects.Remove(grantedEffect);
        GrantedEffectsWithDuration.Remove(grantedEffect);
    }

    private void Update()
    {
        if (GrantedEffectsWithDuration.Count < 1)
            return;

        foreach (var effect in GrantedEffectsWithDuration)
        {
            effect.Update();
        }

        int removed = GrantedEffectsWithDuration.RemoveAll(
            spec =>
            spec.effect.DurationType == EffectDurationType.Duration &&
            spec.CurrentDuration == 0f);

        if (removed > 0)
        {
            GrantedEffects.RemoveAll(
            spec =>
            spec.effect.DurationType == EffectDurationType.Duration &&
            spec.CurrentDuration == 0f);
        }
    }
}
