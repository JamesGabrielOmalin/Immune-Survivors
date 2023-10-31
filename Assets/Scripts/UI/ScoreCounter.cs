using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ScoreCounter : MonoBehaviour
{
    public TMP_Text text;
    public int countFPS = 60;
    public float duration = 2.5f;
    public string numberFormat = "N0";
    private int _value;
    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            UpdateText(value);
            _value = value;
        }
    }
    private Coroutine countCoroutine;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void UpdateText(int newValue)
    {
        if(countCoroutine != null)
        {
            StopCoroutine(countCoroutine);
        }

        countCoroutine = StartCoroutine(CountText(newValue));
    }

    private IEnumerator CountText(int newValue)
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1f / countFPS);
        int previousValue = _value;
        int stepAmount;

        if(newValue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newValue - previousValue) / (countFPS * duration));
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newValue - previousValue) / (countFPS * duration));
        }

        if (previousValue < newValue)
        {
            while (previousValue < newValue)
            {
                previousValue += stepAmount;
                if (previousValue > newValue)
                {
                    previousValue = newValue;
                }

                text.SetText(previousValue.ToString(numberFormat));

                Debug.Log("Score: " + previousValue);

                yield return wait;
            }
        }
        else
        {
            while(previousValue > newValue)
            {
                previousValue += stepAmount;
                if(previousValue < newValue)
                {
                    previousValue = newValue;
                }

                text.SetText(previousValue.ToString(numberFormat));

                Debug.Log("Score: " + previousValue);

                yield return wait;
            }
        }
    }
}
