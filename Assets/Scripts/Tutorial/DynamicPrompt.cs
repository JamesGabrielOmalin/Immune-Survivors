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
        this.titleText.text = title;
        this.descriptionText.text = description;
    }


    public void ActivatePrompt()
    {
        gameObject.SetActive(true);
    }

    public void DeactivatePrompt()
    {
        gameObject.SetActive(false);
    }
}
