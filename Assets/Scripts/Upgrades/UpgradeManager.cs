using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public System.Action OnEffectAcquired;
    public System.Action OnUpgradeScreen;
    public System.Action OnUltiGet;

    [SerializeField] private UpgradeSelect upgradeScreen;
    public bool IsUpgradeScreenOpen => upgradeScreen.gameObject.activeInHierarchy;

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
    public readonly Dictionary<Effect, int> grantedDefaultWeapons = new();

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
        //grantedDefaultWeapons.Add(defaultWeapons[(int)Player.toSpawn], 1);

        grantedEffects[PlayerUnitType.Neutrophil].Add(defaultWeapons[0], 1);
        grantedEffects[PlayerUnitType.Macrophage].Add(defaultWeapons[1], 1);
        grantedEffects[PlayerUnitType.Dendritic].Add(defaultWeapons[2], 1);

        OnEffectAcquired?.Invoke();
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void AddUpgrade(Effect effect, PlayerUnitType unit)
    {
        //switch (effect.EffectType)
        //{
        //    case EffectType.Buff:
        //        if(!grantedEffects[unit].ContainsKey(effect))
        //            grantedEffects[unit].Add(effect, 1);
        //        else
        //            grantedEffects[unit][effect] += 1;
        //        break;
        //    case EffectType.Weapon:
        //        if (defaultWeapons.Contains(effect))
        //        {
        //            if (!grantedDefaultWeapons.ContainsKey(effect))
        //                grantedDefaultWeapons.Add(effect, 1);
        //            else
        //                grantedDefaultWeapons[effect] += 1;
        //        }
        //        else if (!grantedWeapons.ContainsKey(effect))
        //            grantedWeapons.Add(effect, 1);
        //        else
        //            grantedWeapons[effect] += 1;
        //        break;
        //}

        if (!grantedEffects[unit].ContainsKey(effect))
            grantedEffects[unit].Add(effect, 1);
        else
            grantedEffects[unit][effect]++;

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

        if (grantedEffects[PlayerUnitType.Neutrophil].Count >= 4)
        {
            n = new(grantedEffects[PlayerUnitType.Neutrophil].Keys);
        }
        if (grantedEffects[PlayerUnitType.Macrophage].Count >= 4)
        {
            m = new(grantedEffects[PlayerUnitType.Macrophage].Keys);
        }
        if (grantedEffects[PlayerUnitType.Dendritic].Count >= 4)
        {
            d = new(grantedEffects[PlayerUnitType.Dendritic].Keys);
        }

        //if (CanEquipWeapons)
        //{
        //    n.AddRange(neutrophilWeapons);
        //    n.Add(defaultWeapons[0]);
        //    m.AddRange(macrophageWeapons);
        //    m.Add(defaultWeapons[1]);
        //    d.AddRange(dendriticWeapons);
        //    d.Add(defaultWeapons[2]);
        //}
        //else
        //{
        //    n.AddRange(neutrophilWeapons.Intersect(grantedWeapons.Keys.ToList()));
        //    if (grantedDefaultWeapons.ContainsKey(defaultWeapons[0]))
        //    {
        //        n.Add(defaultWeapons[0]);
        //    }
        //    m.AddRange(macrophageWeapons.Intersect(grantedWeapons.Keys.ToList()));
        //    if (grantedDefaultWeapons.ContainsKey(defaultWeapons[1]))
        //    {
        //        m.Add(defaultWeapons[1]);
        //    }
        //    d.AddRange(dendriticWeapons.Intersect(grantedWeapons.Keys.ToList()));
        //    if (grantedDefaultWeapons.ContainsKey(defaultWeapons[2]))
        //    {
        //        d.Add(defaultWeapons[2]);
        //    }
        //}

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
        AudioManager.instance.Play("UpgradePrompt", transform.position);
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
