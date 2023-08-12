using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public System.Action OnEffectAcquired;

    [SerializeField] private UpgradeSelect upgradeScreen;

    [Header("Buffs")]
    [SerializeField] private List<Effect> neutrophilUpgrades = new();
    [SerializeField] private List<Effect> macrophageUpgrades = new();
    [SerializeField] private List<Effect> dendriticUpgrades = new();

    [Header("Weapons")]
    [SerializeField] private List<Effect> neutrophilWeapons = new();
    [SerializeField] private List<Effect> macrophageWeapons= new();
    [SerializeField] private List<Effect> dendriticWeapons = new();

    [SerializeField] private List<Effect> defaultWeapons = new();

    public readonly Dictionary<PlayerUnitType, Dictionary<Effect, int>> grantedEffects = new()
    {
        { PlayerUnitType.Neutrophil, new() },
        { PlayerUnitType.Macrophage, new() },
        { PlayerUnitType.Dendritic, new() },
    };

    //private readonly List<Effect> grantedWeapons = new();
    public readonly Dictionary<Effect, int> grantedWeapons = new();

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

    private void Start()
    {
        grantedWeapons.Add(defaultWeapons[(int)Player.toSpawn], 1);

        OnEffectAcquired?.Invoke();
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void AddUpgrade(Effect effect, PlayerUnitType unit)
    {
        switch (effect.EffectType)
        {
            case EffectType.Buff:
                if(!grantedEffects[unit].ContainsKey(effect))
                    grantedEffects[unit].Add(effect, 1);
                else
                    grantedEffects[unit][effect] += 1;
                break;
            case EffectType.Weapon:
                if (!grantedWeapons.ContainsKey(effect))
                    grantedWeapons.Add(effect, 1);
                else
                    grantedWeapons[effect] += 1;
                break;
        }

        OnEffectAcquired?.Invoke();
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
            n.AddRange(neutrophilWeapons.Intersect(grantedWeapons.Keys.ToList()));
            m.AddRange(macrophageWeapons.Intersect(grantedWeapons.Keys.ToList()));
            d.AddRange(dendriticWeapons.Intersect(grantedWeapons.Keys.ToList()));
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
        GameManager.instance.Player.GetComponent<Player>().EnableHUD(false);
        upgradeScreen.SelectUpgrades(type);
        upgradeScreen.gameObject.SetActive(true);
    }

    public Dictionary<Effect, int> GetEffects(PlayerUnitType type)
    {
        return grantedEffects[type];
    }

    public Dictionary<Effect, int> GetWeapons()
    {
        return grantedWeapons;
    }

    public void OnDisable()
    {
        OnEffectAcquired = null;
    }
}
