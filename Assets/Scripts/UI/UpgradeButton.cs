using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public Effect Upgrade { get; private set; }
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text effectDescriptionText;

    [SerializeField] private Image icon;
    public Image unitIcon;
    public List<Sprite> unitSprites = new();
    [HideInInspector] public PlayerUnitType type;

    private void Start()
    {

    }

    private void OnEnable()
    {
        unitIcon.sprite = unitSprites[(int)type];
    }

    public void SetUpgrade(in Effect inUpgrade)
    {
        Upgrade = inUpgrade;
        nameText.text = inUpgrade.Name + $" <size=18><color=yellow>({inUpgrade.EffectType})</size></color>";
        descriptionText.text = inUpgrade.Description;
        if (inUpgrade.EffectType == EffectType.Weapon)
        {
            if (UpgradeManager.instance.grantedWeapons.ContainsKey(inUpgrade))
            {
                int level = Mathf.Min(UpgradeManager.instance.grantedWeapons[inUpgrade], 4);
                nameText.text = inUpgrade.Name + $" Lv.{level + 1} <size=18><color=yellow>({inUpgrade.EffectType})</size></color>";
                effectDescriptionText.text = inUpgrade.EffectDescriptions[level];
            }
            else
            {
                nameText.text = "<sup><size=24><color=yellow>NEW!</sup></size></color> " + nameText.text;
                effectDescriptionText.text = inUpgrade.EffectDescriptions[0];
            }
        }
        else
        {
            if (!UpgradeManager.instance.grantedEffects[type].ContainsKey(inUpgrade))
                nameText.text = "<sup><size=24><color=yellow>NEW!</sup></size></color> " + nameText.text;
                
            effectDescriptionText.text = inUpgrade.EffectDescription;
        }

        icon.sprite = inUpgrade.Sprite;
    }
}
