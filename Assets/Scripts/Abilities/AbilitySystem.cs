using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem : MonoBehaviour
{
    public List<AbilitySpec> GrantedAbilities = new();

    public void GrantAbility(AbilitySpec spec)
    {
        GrantedAbilities.Add(spec);
    }
}
