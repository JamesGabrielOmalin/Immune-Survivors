using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private ScoreCounter winScoreCounter;
    [SerializeField] private ScoreCounter loseScoreCounter;

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
        GameManager.instance.OnGameWin -= ShowWinScreen;

        GameManager.instance.HUD.SetActive(false);
        TutorialManager.instance.dynamicPrompt.SetActive(false);
        GameManager.instance.Player.GetComponent<Player>().activeHUD.SetActive(false);
        GameManager.instance.Player.GetComponent<Player>().OnEnableBuffHUD(false);
        winScreen.SetActive(true);
        winScoreCounter.Value = calculateScore();
        GameManager.instance.PauseGame();
        GameManager.instance.PauseGameTime();
    }

    private void ShowLoseScreen()
    {
        GameManager.instance.OnGameLose -= ShowLoseScreen;

        GameManager.instance.HUD.SetActive(false);
        TutorialManager.instance.dynamicPrompt.SetActive(false);
        GameManager.instance.Player.GetComponent<Player>().activeHUD.SetActive(false);
        GameManager.instance.Player.GetComponent<Player>().OnEnableBuffHUD(false);
        loseScreen.SetActive(true);
        loseScoreCounter.Value = calculateScore();
        GameManager.instance.PauseGame();
        GameManager.instance.PauseGameTime();
    }

    private int calculateScore()
    {
        int score;

        score = ((AntigenManager.instance.GetAntigenCount(AntigenType.Type_1) +
                  AntigenManager.instance.GetAntigenCount(AntigenType.Type_2) +
                  AntigenManager.instance.GetAntigenCount(AntigenType.Type_3))) + (RecruitManager.instance.totalKillCount * 10);

        return score;
    }
}
