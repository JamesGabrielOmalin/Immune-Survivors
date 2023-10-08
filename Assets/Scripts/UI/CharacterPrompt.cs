using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterPrompt : MonoBehaviour
{
    [SerializeField] private GameObject[] prompts;

    public bool isToggle = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //  Method that switches the text that is shown up every button click (alternating between the two)
    public void ToggleText()
    {
        prompts[Convert.ToInt32(isToggle)].SetActive(false);
        isToggle = !isToggle;
        prompts[Convert.ToInt32(isToggle)].SetActive(true);
    }
}
