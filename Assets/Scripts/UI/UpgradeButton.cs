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

    private void Start()
    {

    }

    public void SetUpgrade(Effect inUpgrade)
    {
        Upgrade = inUpgrade;
        nameText.text = inUpgrade.Name;
        descriptionText.text = inUpgrade.Description;
    }
}
