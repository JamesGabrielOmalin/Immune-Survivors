using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [SerializeField] private UpgradeSelect upgradeScreen;

    [Header("Buffs")]
    [SerializeField] private List<Effect> neutrophilUpgrades = new();
    [SerializeField] private List<Effect> macrophageUpgrades = new();
    [SerializeField] private List<Effect> dendriticUpgrades = new();

    [Header("Weapons")]
    [SerializeField] private List<Effect> neutrophilWeapons = new();
    [SerializeField] private List<Effect> macrophageWeapons= new();
    [SerializeField] private List<Effect> dendriticWeapons = new();

    private readonly Dictionary<PlayerUnitType, List<Effect>> grantedEffects = new()
    {
        { PlayerUnitType.Neutrophil, new() },
        { PlayerUnitType.Macrophage, new() },
        { PlayerUnitType.Dendritic, new() },
    };
    private readonly List<Effect> grantedWeapons = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    public void AddUpgrade(Effect effect, PlayerUnitType unit)
    {
        switch (effect.EffectType)
        {
            case EffectType.Buff:
                grantedEffects[unit].Add(effect);
                break;
            case EffectType.Weapon:
                grantedWeapons.Add(effect);
                break;
        }
    }

    public bool CanEquipWeapons => grantedWeapons.Count < 3;

    public bool CanGetBuff(PlayerUnitType type)
    {
        return grantedEffects[type].Count < 3;
    }

    public Effect[] GetRandomUpgrades(PlayerUnitType type)
    {
        List<Effect> n = new(neutrophilUpgrades);
        List<Effect> m = new(macrophageUpgrades);
        List<Effect> d = new(dendriticUpgrades);

        if (CanEquipWeapons)
        {
            n.AddRange(neutrophilWeapons);
            m.AddRange(macrophageWeapons);
            d.AddRange(dendriticWeapons);
        }
        else
        {
            n.AddRange(neutrophilWeapons.Intersect(grantedWeapons));
            m.AddRange(macrophageWeapons.Intersect(grantedWeapons));
            d.AddRange(dendriticWeapons.Intersect(grantedWeapons));
        }

        return type switch
        {
            PlayerUnitType.Neutrophil => n.GenerateRandom(3).ToArray(),
            PlayerUnitType.Macrophage => m.GenerateRandom(3).ToArray(),
            PlayerUnitType.Dendritic => d.GenerateRandom(3).ToArray(),
            _ => null,
        };
    }

    public void OpenUpgradeScreen(PlayerUnitType type)
    {
        GameManager.instance.PauseGameTime();
        GameManager.instance.HUD.SetActive(false);
        upgradeScreen.SelectUpgrades(type);
        upgradeScreen.gameObject.SetActive(true);
    }
}
