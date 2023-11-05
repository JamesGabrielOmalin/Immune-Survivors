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

    private void Start()
    {

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
                effectDescriptionText.text = inUpgrade.EffectDescriptions[Mathf.Min(UpgradeManager.instance.grantedWeapons[inUpgrade], 4)];
            }
            else
            {
                nameText.text = "<sup><size=24><color=yellow>NEW!</sup></size></color> " + nameText.text;
                effectDescriptionText.text = inUpgrade.EffectDescriptions[0];
            }
        }
        else
        {
            effectDescriptionText.text = inUpgrade.EffectDescription;
        }

        icon.sprite = inUpgrade.Sprite;
    }
}
