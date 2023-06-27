using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AntigenDisplay : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> antigens = new();

    private void Start()
    {
        AntigenManager.instance.OnAntigenCountChanged[AntigenType.Type_1] += (() => UpdateText(AntigenType.Type_1));
        AntigenManager.instance.OnAntigenCountChanged[AntigenType.Type_2] += (() => UpdateText(AntigenType.Type_2));
        AntigenManager.instance.OnAntigenCountChanged[AntigenType.Type_3] += (() => UpdateText(AntigenType.Type_3));
    }

    private void UpdateText(AntigenType type)
    {
        antigens[(int)type].text = AntigenManager.instance.GetAntigenCount(type).ToString();
    }
}
