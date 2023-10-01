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
    [SerializeField] private Image icon;
    public Image unitIcon;
    public List<Sprite> unitSprites = new();

    private void Start()
    {

    }

    public void SetUpgrade(Effect inUpgrade)
    {
        Upgrade = inUpgrade;
        nameText.text = inUpgrade.Name;

        if (inUpgrade.EffectType == EffectType.Weapon)
        {

            if (UpgradeManager.instance.grantedDefaultWeapons.ContainsKey(inUpgrade))
            {
                descriptionText.text = inUpgrade.Descriptions[Mathf.Min(UpgradeManager.instance.grantedDefaultWeapons[inUpgrade], 4)];
            }
            else if (UpgradeManager.instance.grantedWeapons.ContainsKey(inUpgrade))
            {
                descriptionText.text = inUpgrade.Descriptions[Mathf.Min(UpgradeManager.instance.grantedWeapons[inUpgrade], 4)];
            }
            else
            {
                descriptionText.text = inUpgrade.Descriptions[0];
            }
        }
        else
        {
            descriptionText.text = inUpgrade.Description;
        }

        icon.sprite = inUpgrade.Sprite;
    }
}
