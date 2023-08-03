using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        StartCoroutine(UpdateDisplay());
    }

    private IEnumerator UpdateDisplay()
    {
        yield return null;

        while(this)
        {
            text.text = GameManager.instance.GameTimeDisplay;
            yield return new WaitForSeconds(1f);
        }
    }
}
