using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UpgradeManager : MonoBehaviour
{
    [Serializable]
    public class BonusEffect
    {
        public int level;
        public Effect effect;
    }
    public static UpgradeManager instance;

    [SerializeField] private UpgradeSelect upgradeScreen;


    [Header ("Neutrophil")]
    [SerializeField] private List<Effect> neutrophilUpgrades = new();
    [SerializeField] private List<BonusEffect> neutrophilBonusUpgrades = new();

    [Header("Macrophage")]
    [SerializeField] private List<Effect> macrophageUpgrades = new();
    [SerializeField] private List<BonusEffect> macrophageBonusUpgrades = new();

    [Header("Dendritic")]
    [SerializeField] private List<Effect> dendriticUpgrades = new();
    [SerializeField] private List<BonusEffect> dendriticBonusUpgrades = new();


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

    public List<Effect> GetBonusUpgrades(PlayerUnitType type)
    {    
        List<BonusEffect> bonusEffectsPool = null;

        switch (type)
        {
            case PlayerUnitType.Neutrophil:
                bonusEffectsPool = neutrophilBonusUpgrades;
                break;

            case PlayerUnitType.Macrophage:
                bonusEffectsPool = macrophageBonusUpgrades;
                break;

            case PlayerUnitType.Dendritic:
                bonusEffectsPool = dendriticBonusUpgrades;
                break;
        }


        float playerLevel = GameManager.instance.Player.GetComponent<Player>().GetUnit(type).GetComponent<AttributeSet>().GetAttribute("Level").Value;

        List<Effect> applicableBonusEffects = new();

        if (bonusEffectsPool.Count>0)
        {

            foreach (var bonus in bonusEffectsPool) 
            {
                // return the effects of the level bonus effect
                if(bonus.level == playerLevel)
                {
                    applicableBonusEffects.Add(bonus.effect);
                }

            }

            return applicableBonusEffects;
        }
        else
        {
            return null;
        }


    }

    public void OpenUpgradeScreen(PlayerUnitType type)
    {
        GameManager.instance.PauseGameTime();
        upgradeScreen.SelectUpgrades(type);
        upgradeScreen.gameObject.SetActive(true);
    }
}
