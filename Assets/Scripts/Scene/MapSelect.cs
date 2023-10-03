using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int selectedLevel = 1;

    private const int MAX_LEVEL = 3;

    public void NextLevel()
    {
        selectedLevel++;

        nextButton.interactable = selectedLevel < MAX_LEVEL;
        previousButton.interactable = true;
    }

    public void PreviousLevel()
    {
        selectedLevel--;

        nextButton.interactable = true;
        previousButton.interactable = selectedLevel > 1;
    }

    public void LoadLevel()
    {
        SceneLoader.instance.LoadScene(selectedLevel);
    }
}
