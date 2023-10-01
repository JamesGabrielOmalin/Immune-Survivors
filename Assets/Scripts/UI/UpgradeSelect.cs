using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelect : MonoBehaviour
{
    [SerializeField] private List<UpgradeButton> buttons = new();
    
    public PlayerUnitType UnitToUpgrade { get; private set; }

    public void SelectUpgrades(PlayerUnitType type)
    {
        UnitToUpgrade = type;

        var effects = UpgradeManager.instance.GetRandomUpgrades(UnitToUpgrade);

        for (int i = 0; i < effects.Length; i++)
        {
            buttons[i].SetUpgrade(effects[i]);
            buttons[i].unitIcon.sprite = buttons[i].unitSprites[(int)type];
        }
    }

    public void ApplyUpgrade(int index)
    {
        //var playerAbilitySystem = GameManager.instance.Player.GetComponent<Player>().GetUnit(UnitToUpgrade).GetComponent<AbilitySystem>();
        //playerAbilitySystem.ApplyEffectToSelf(buttons[index].Upgrade);

        Effect upgrade = buttons[index].Upgrade;
        UpgradeManager.instance.AddUpgrade(upgrade, UnitToUpgrade);

        GameManager.instance.HUD.SetActive(true);
        GameManager.instance.Player.GetComponent<Player>().EnableHUD(true);
        GameManager.instance.Player.GetComponent<Player>().GetUnit(UnitToUpgrade).AddUpgrade(upgrade);
        GameManager.instance.ResumeGameTime();
    }
}
