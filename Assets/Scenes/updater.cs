using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class updater : MonoBehaviour
{
    public ScoreCounter scoreCounter;
    public TMP_InputField InputField;

    public void SetValue()
    {
        scoreCounter.gameObject.SetActive(true);

        int value;

        if (int.TryParse(InputField.text, out value))
        {
            scoreCounter.Value = value;
        }
    }
}
