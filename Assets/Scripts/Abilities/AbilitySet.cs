using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.UI;

public class AbilitySet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AbilitySystem abilitySystem;

    [Header("Abilities")]
    [SerializeField] private Ability basicAttackAbility;
    [SerializeField] private Ability mobilityAbility;
    [SerializeField] private Ability ultimateAbility;

    public AbilitySpec BasicAttack { get; private set; }
    public AbilitySpec Mobility { get; private set; }
    public AbilitySpec Ultimate { get; private set; }

    [Header("UI")]
    [SerializeField] private Image mobilityCDIcon;
    [SerializeField] private Image ultimateCDIcon;
    [SerializeField] private Image ultimateBlockIcon;

    private void Start()
    {
        if (basicAttackAbility)
        {
            BasicAttack = basicAttackAbility.CreateSpec(this.abilitySystem);
            abilitySystem.GrantAbility(BasicAttack);
        }

        if (mobilityAbility)
        {
            Mobility = mobilityAbility.CreateSpec(this.abilitySystem);
            abilitySystem.GrantAbility(Mobility);
        }

        if (ultimateAbility)
        {
            Ultimate = ultimateAbility.CreateSpec(this.abilitySystem);
            abilitySystem.GrantAbility(Ultimate);
        }
    }

    //private void FixedUpdate()
    //{
    //    if (IsAI)
    //        return;

    //    if (BasiC != null && basicAttackCDIcon != null)
    //    {
    //        basicAttackCDIcon.fillAmount = basicAttackAbilitySpec.CurrentCD / basicAttackAbility.Cooldown;
    //    }

    //    if (mobilityAbilitySpec != null && mobilityCDIcon != null)
    //    {
    //        mobilityCDIcon.fillAmount = mobilityAbilitySpec.CurrentCD / mobilityAbility.Cooldown;
    //    }

    //    if (ultimateAbilitySpec != null && ultimateCDIcon != null)
    //    {
    //        ultimateCDIcon.fillAmount = ultimateAbilitySpec.CurrentCD / ultimateAbility.Cooldown;
    //    }
    //}

    //public void Ultimate(InputAction.CallbackContext context)
    //{
    //    //StartCoroutine(ProcessUlimate());
    //    StartCoroutine(ultimateAbilitySpec.TryActivateAbility());
    //}

    //public void Mobility(InputAction.CallbackContext context)
    //{
    //    //StartCoroutine(ProcessMobility());
    //    StartCoroutine(mobilityAbilitySpec.TryActivateAbility());
    //}

    //public void BasicAttack()
    //{
    //    //StartCoroutine(ProcessMobility());
    //    StartCoroutine(basicAttackAbilitySpec.TryActivateAbility());
    //}

    //private void CheckUltimateAvailbility(GameObject unit)
    //{
    //    var abilitySet = unit.GetComponent<AbilitySet>();

    //    if (abilitySet.IsAI)
    //    {
    //        return;
    //    }

    //    if (abilitySet.ultimateAbilitySpec.CanActivateAbility())
    //    {
    //        abilitySet.ultimateBlockIcon.gameObject.SetActive(false);
    //    }
    //}
}
