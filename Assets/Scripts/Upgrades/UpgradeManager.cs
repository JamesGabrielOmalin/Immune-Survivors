using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [SerializeField] private UpgradeSelect upgradeScreen;

    [SerializeField] private List<Effect> neutrophilUpgrades = new();
    [SerializeField] private List<Effect> macrophageUpgrades = new();
    [SerializeField] private List<Effect> dendriticUpgrades = new();

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

    public Effect[] GetRandomUpgrades(PlayerUnitType type)
    {
        Player player = GameManager.instance.Player.GetComponent<Player>();
        PlayerUnit unit = player.GetUnit(type);

        return type switch
        {
            PlayerUnitType.Neutrophil => neutrophilUpgrades.Where(x => (x.EffectType == EffectType.Weapon && player.CanEquipWeapon()) || (x.EffectType == EffectType.Buff && unit.CanBeUpgraded())).ToList().GenerateRandom(3).ToArray(),
            PlayerUnitType.Macrophage => macrophageUpgrades.Where(x => (x.EffectType == EffectType.Weapon && player.CanEquipWeapon()) || (x.EffectType == EffectType.Buff && unit.CanBeUpgraded())).ToList().GenerateRandom(3).ToArray(),
            PlayerUnitType.Dendritic => dendriticUpgrades.Where(x => (x.EffectType == EffectType.Weapon && player.CanEquipWeapon()) || (x.EffectType == EffectType.Buff && unit.CanBeUpgraded())).ToList().GenerateRandom(3).ToArray(),
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
