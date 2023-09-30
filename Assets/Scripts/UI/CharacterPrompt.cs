using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterPrompt : MonoBehaviour
{
    [SerializeField] private GameObject[] texts;
    [SerializeField] private GameObject[] images;

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
        texts[Convert.ToInt32(isToggle)].SetActive(false);
        images[Convert.ToInt32(isToggle)].SetActive(false);
        isToggle = !isToggle;
        texts[Convert.ToInt32(isToggle)].SetActive(true);
        images[Convert.ToInt32(isToggle)].SetActive(true);
    }
}
