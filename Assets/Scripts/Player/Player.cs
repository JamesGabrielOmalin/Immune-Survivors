using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput input;
    [Header("Units")]
    [SerializeField] private PlayerUnit neutrophil;
    [SerializeField] private PlayerUnit macrophage;
    [SerializeField] private PlayerUnit dendritic;

    [Header("Slots")]
    [SerializeField] private Transform activeSlot;
    [SerializeField] private Transform slot1;
    [SerializeField] private Transform slot2;

    private PlayerUnit activeUnit;
    private int numRecruit;

    public static PlayerUnitType toSpawn;

    private Dictionary<PlayerUnitType, bool> unitRecruited = new()
    {
        { PlayerUnitType.Neutrophil, false },
        { PlayerUnitType.Macrophage, false },
        { PlayerUnitType.Dendritic, false },
    };

    private void Awake()
    {
        // TODO: Implement character select?
        switch (toSpawn)
        {
            case PlayerUnitType.Neutrophil:
                RecruitUnit(neutrophil);
                activeUnit = neutrophil;
                break;
            case PlayerUnitType.Macrophage:
                RecruitUnit(macrophage);
                activeUnit = macrophage;
                break;
            case PlayerUnitType.Dendritic:
                RecruitUnit(dendritic);
                activeUnit = dendritic;
                break;
        }
    }

    private void Start()
    {
        input.Controls.Abilities.Mobility.started += (ctx) => StartCoroutine(activeUnit.AbilitySet.Mobility.TryActivateAbility());
        input.Controls.Abilities.Ultimate.started += (ctx) => StartCoroutine(activeUnit.AbilitySet.Ultimate.TryActivateAbility());
    }

    public void RecruitUnit(PlayerUnit recruit)
    {
        // If already recruit, upgrade instead
        if (unitRecruited[recruit.UnitType])
        {
            UpgradeUnit(recruit.UnitType);
            return;
        }

        Transform slot = null;

        switch (numRecruit)
        {
            case 0:
                slot = activeSlot;
                break;
            case 1:
                slot = slot1;
                break;
            case 2:
                slot = slot2;
                break;
        }

        switch (recruit.UnitType)
        {
            case PlayerUnitType.Neutrophil:
                neutrophil.gameObject.SetActive(true);
                neutrophil.transform.parent = slot;
                neutrophil.transform.localPosition = Vector3.zero;
                break;
            case PlayerUnitType.Macrophage:
                macrophage.gameObject.SetActive(true);
                macrophage.transform.parent = slot;
                macrophage.transform.localPosition = Vector3.zero;
                break;
            case PlayerUnitType.Dendritic:
                dendritic.gameObject.SetActive(true);
                dendritic.transform.parent = slot;
                dendritic.transform.localPosition = Vector3.zero;
                break;
        }

        unitRecruited[recruit.UnitType] = true;

        numRecruit++;
    }

    private void UpgradeUnit(PlayerUnitType unitType)
    {
        switch (unitType)
        {
            case PlayerUnitType.Neutrophil:
                neutrophil.Upgrade();
                break;
            case PlayerUnitType.Macrophage:
                macrophage.Upgrade();
                break;
            case PlayerUnitType.Dendritic:
                dendritic.Upgrade();
                break;
        }
    }

    public PlayerUnit GetActiveUnit()
    {
        return activeUnit;
    }

    public void ApplyBuffs(AntigenType type, AttributeModifier mod, float duration)
    {
        StartCoroutine(BuffCoroutine(type, mod, duration));
    }

    private IEnumerator BuffCoroutine(AntigenType type, AttributeModifier mod, float duration)
    {
        var n = neutrophil.attributes.GetAttribute(type.ToString() + " DMG Bonus");
        var m = macrophage.attributes.GetAttribute(type.ToString() + " DMG Bonus");
        var d = dendritic.attributes.GetAttribute(type.ToString() + " DMG Bonus");
        n.AddModifier(mod);
        m.AddModifier(mod);
        d.AddModifier(mod);

        yield return new WaitForSeconds(duration);

        n.RemoveModifier(mod);
        m.RemoveModifier(mod);
        d.RemoveModifier(mod);
    }

    public PlayerUnit GetUnit(PlayerUnitType type)
    {
        return type switch
        {
            PlayerUnitType.Neutrophil => neutrophil,
            PlayerUnitType.Macrophage => macrophage,
            PlayerUnitType.Dendritic => dendritic,
            _ => null,
        };
    }
}
