using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelect : MonoBehaviour
{
    [SerializeField] private List<UpgradeButton> buttons = new();
    
    public PlayerUnitType UnitToUpgrade { get; private set; }

    // Start is called before the first frame update
    private void OnEnable()
    {
        //PlayerUnitType type = GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().UnitType;
        //var effects = UpgradeManager.instance.GetRandomUpgrades(type);

        //for (int i = 0; i < effects.Length; i++)
        //{
        //    buttons[i].SetUpgrade(effects[i]);
        //}
    }

    public void SelectUpgrades(PlayerUnitType type)
    {
        UnitToUpgrade = type;

        var effects = UpgradeManager.instance.GetRandomUpgrades(UnitToUpgrade);

        for (int i = 0; i < effects.Length; i++)
        {
            buttons[i].SetUpgrade(effects[i]);
        }
    }

    public void ApplyUpgrade(int index)
    {
        var playerAbilitySystem = GameManager.instance.Player.GetComponent<Player>().GetUnit(UnitToUpgrade).GetComponent<AbilitySystem>();
        playerAbilitySystem.ApplyEffectToSelf(buttons[index].Upgrade);

        GameManager.instance.ResumeGameTime();
    }
}
