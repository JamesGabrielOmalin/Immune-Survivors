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
        return type switch
        {
            PlayerUnitType.Neutrophil => neutrophilUpgrades.GenerateRandom(3).ToArray(),
            PlayerUnitType.Macrophage => macrophageUpgrades.GenerateRandom(3).ToArray(),
            PlayerUnitType.Dendritic => dendriticUpgrades.GenerateRandom(3).ToArray(),
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
