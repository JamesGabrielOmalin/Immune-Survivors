using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicPrompt : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
