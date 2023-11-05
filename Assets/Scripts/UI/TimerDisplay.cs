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
            if (420 - GameManager.instance.GameTime.TotalSeconds >= 0)
            {
                text.text = GameManager.instance.GameTimeDisplay;
            }
            else
            {
                text.text = "<color=red>LAST STAND</color>";
                yield break;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
