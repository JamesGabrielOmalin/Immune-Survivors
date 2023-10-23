using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicPrompt : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    public void SetText(string title, string description)
    {
        Debug.Log("PROMPT");
        this.titleText.text = title;
        this.descriptionText.text = description;
    }
}
