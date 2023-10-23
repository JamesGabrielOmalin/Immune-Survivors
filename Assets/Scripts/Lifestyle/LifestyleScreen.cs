using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifestyleScreen : MonoBehaviour
{
    [SerializeField] private List<UpgradeButton> buttons = new();

    public void SelectLifestyles()
    {
        var effects = LifestyleManager.instance.GetRandomLifestyles();

        for (int i = 0; i < effects.Length; i++)
        {
            buttons[i].SetUpgrade(effects[i]);
        }
    }

    public void ApplyLifeStyle(int index)
    {
        Effect upgrade = buttons[index].Upgrade;
        LifestyleManager.instance.AddLifestyle(upgrade);

        GameManager.instance.HUD.SetActive(true);
        var player = GameManager.instance.Player.GetComponent<Player>();
        player.EnableHUD(true);
        player.GetUnit(PlayerUnitType.Neutrophil).AddUpgrade(upgrade);
        player.GetUnit(PlayerUnitType.Macrophage).AddUpgrade(upgrade);
        player.GetUnit(PlayerUnitType.Dendritic).AddUpgrade(upgrade);

        GameManager.instance.ResumeGameTime();
    }
}
