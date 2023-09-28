using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text killCountText;
    // Start is called before the first frame update
    void Start()
    {
        RecruitManager.instance.OnKillCountUpdate += UpdateKillCount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateKillCount()
    {
        if (killCountText != null) 
        {
            killCountText.text = RecruitManager.instance.totalKillCount.ToString();
        }
    }
}
