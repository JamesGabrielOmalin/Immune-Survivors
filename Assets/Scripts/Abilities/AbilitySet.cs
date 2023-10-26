using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private TMP_Text mobilityCDText;
    [SerializeField] private TMP_Text ultimateCDText;
    [SerializeField] private GameObject mobilityCharges;
    [SerializeField] private List<Image> mobilityChargeIcons;

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
            Mobility.OnAbilityCooldownStart += delegate { mobilityCDText.gameObject.SetActive(true); };
            Mobility.OnAbilityCooldownEnd += delegate { mobilityCDText.gameObject.SetActive(false); };

            mobilityCharges.SetActive(mobilityAbility.HasCharges);

            Mobility.OnChargeGained += UpdateMobilityCharges;
            Mobility.OnChargeLost += UpdateMobilityCharges;
        }
    }

    private void UpdateMobilityCharges()
    {
        foreach (var icon in mobilityChargeIcons)
        {
            icon.enabled = false;
        }

        for (int i = 0; i < Mobility.CurrentCharge; i++)
        {
            mobilityChargeIcons[i].enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (Mobility != null)
        {
            if (mobilityCDIcon != null)
                mobilityCDIcon.fillAmount = Mobility.CurrentCD / mobilityAbility.Cooldown;
            if (mobilityCDText != null && mobilityCDText.enabled)
                mobilityCDText.text = $"{Mobility.CurrentCD:0.0}";
        }

        if (Ultimate != null)
        {
            if (ultimateCDIcon != null)
                ultimateCDIcon.fillAmount = Ultimate.CurrentCD / ultimateAbility.Cooldown;
            if (ultimateCDText != null && ultimateCDText.enabled)
                ultimateCDText.text = $"{Ultimate.CurrentCD:0.0}";
        }
    }

    public void EnableBasicAttack(bool enabled)
    {
        CanUseBasicAttack = enabled;
    }

    public void GrantUltimate()
    {
        PlayerUnit player = GetComponent<PlayerUnit>();
        if (ultimateAbility && player.UnitType == Player.toSpawn)
        {
            ultimateButton.SetActive(true);
            Ultimate = ultimateAbility.CreateSpec(this.abilitySystem);
            abilitySystem.GrantAbility(Ultimate);
            Ultimate.OnAbilityCooldownStart += delegate { ultimateCDText.gameObject.SetActive(true); };
            Ultimate.OnAbilityCooldownEnd += delegate { ultimateCDText.gameObject.SetActive(false); };
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
