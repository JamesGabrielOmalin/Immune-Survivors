using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image mapPreview;
    [SerializeField] private Sprite[] previewSprites;

    private int selectedLevel = 1;

    private const int MAX_LEVEL = 3;

    public void NextLevel()
    {
        selectedLevel++;

        nextButton.interactable = selectedLevel < MAX_LEVEL;
        previousButton.interactable = true;

        UpdateText();
        UpdatePreview();
    }

    public void PreviousLevel()
    {
        selectedLevel--;

        nextButton.interactable = true;
        previousButton.interactable = selectedLevel > 1;

        UpdateText();
        UpdatePreview();
    }

    public void LoadLevel()
    {
        SceneLoader.instance.LoadScene(selectedLevel);
    }

    public void UpdateText()
    {
        text.text = "Level " + selectedLevel;
    }

    public void UpdatePreview()
    {
        mapPreview.sprite = previewSprites[selectedLevel-1];
    }
}
