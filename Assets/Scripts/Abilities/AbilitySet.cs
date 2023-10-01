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

    public bool CanUseBasicAttack { get; private set; } = true;

    [Header("UI")]
    [SerializeField] private Image mobilityCDIcon;
    [SerializeField] private Image ultimateCDIcon;
    [SerializeField] private GameObject ultimateButton;

    private void Awake()
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
    }

    private void FixedUpdate()
    {
        if (Mobility != null && mobilityCDIcon != null)
        {
            mobilityCDIcon.fillAmount = Mobility.CurrentCD / mobilityAbility.Cooldown;
        }

        if (Ultimate != null && ultimateCDIcon != null)
        {
            ultimateCDIcon.fillAmount = Ultimate.CurrentCD / ultimateAbility.Cooldown;
        }
    }

    public void EnableBasicAttack(bool enabled)
    {
        CanUseBasicAttack = enabled;
    }

    public void GrantUltimate()
    {
        if (ultimateAbility)
        {
            ultimateButton.SetActive(true);
            Ultimate = ultimateAbility.CreateSpec(this.abilitySystem);
            abilitySystem.GrantAbility(Ultimate);
            UpgradeManager.instance.OnUltiGet?.Invoke();
        }
    }

    public void ActivateUltimate()
    {
        if (Ultimate != null)
            StartCoroutine(Ultimate.TryActivateAbility());
    }

    public void ActivateMobility()
    {
        if (Mobility != null)
            StartCoroutine(Mobility.TryActivateAbility());
    }

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
