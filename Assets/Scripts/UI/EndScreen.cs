using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    // Start is called before the first frame update
    private void Start()
    {
        GameManager.instance.OnGameLose += ShowLoseScreen;
        GameManager.instance.OnGameWin += ShowWinScreen;

        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    private void ShowWinScreen()
    {
        GameManager.instance.HUD.SetActive(false);
        TutorialManager.instance.dynamicPrompt.SetActive(false);
        GameManager.instance.Player.GetComponent<Player>().activeHUD.SetActive(false);
        GameManager.instance.Player.GetComponent<Player>().OnEnableBuffHUD(false);
        winScreen.SetActive(true);
        GameManager.instance.PauseGame();
        GameManager.instance.PauseGameTime();
    }

    private void ShowLoseScreen()
    {
        GameManager.instance.HUD.SetActive(false);
        TutorialManager.instance.dynamicPrompt.SetActive(false);
        GameManager.instance.Player.GetComponent<Player>().activeHUD.SetActive(false);
        GameManager.instance.Player.GetComponent<Player>().OnEnableBuffHUD(false);
        loseScreen.SetActive(true);
        GameManager.instance.PauseGame();
        GameManager.instance.PauseGameTime();
    }
}
