using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class LoadingScreenTip
{
    public string name;
    public string description;
}

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private List<LoadingScreenTip> tips = new();
    [SerializeField] private TMP_Text tipNameText;
    [SerializeField] private TMP_Text tipDescriptionText;

    // Start is called before the first frame update
    private void Start()
    {
        DisplayTip();
    }

    public void DisplayTip()
    {
        LoadingScreenTip tip = tips[Random.Range(0, tips.Count)];
        tipNameText.text = tip.name;
        tipDescriptionText.text = tip.description;
    }
}
