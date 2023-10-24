using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptEnabler : MonoBehaviour
{
    private Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        if (toggle != null)
            toggle.isOn = TutorialManager.isFirstTime;
    }
    public void SetPrompt(bool boolean)
    {
        TutorialManager.isFirstTime = boolean;
    }

    public void EnableGeneralInstructionStaticPrompt()
    {
        TutorialManager.instance.EnablePromptOnPlayerMovement();
    }
}
