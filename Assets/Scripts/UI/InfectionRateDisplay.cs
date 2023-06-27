using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfectionRateDisplay : MonoBehaviour
{
    [SerializeField] private Slider infectionBar;

    private void Start()
    {
        infectionBar.maxValue = EnemyManager.instance.MaxInfectionRate;

        EnemyManager.instance.OnInfectionRateChanged += delegate
        {
            infectionBar.value = EnemyManager.instance.InfectionRate;
        };
    }
}
