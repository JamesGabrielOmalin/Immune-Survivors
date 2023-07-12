using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [field: SerializeField] public GameObject Player { get; private set; }
    [field: SerializeField] public GameObject HUD { get; private set; }

    public System.TimeSpan GameTime { get; private set; }
    public float TimeToWin;

    public bool GameTimePaused { get; private set; }

    public bool GamePaused { get; private set; }

    public System.Action OnGamePaused;
    public System.Action OnGameResumed;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(GameTimer());
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void PauseGameTime()
    {
        Time.timeScale = 0f;
        GameTimePaused = true;
    }
    public void ResumeGameTime()
    {
        if (GamePaused)
            return;
        Time.timeScale = 1f;
        GameTimePaused = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        GamePaused = true;

        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (!GameTimePaused)
            Time.timeScale = 1f;
        GamePaused = false;

        OnGameResumed?.Invoke();
    }

    private IEnumerator GameTimer()
    {
        WaitForSeconds wait = new(1f);
        
        while (GameTime.TotalSeconds < TimeToWin)
        {
            yield return wait;
            GameTime.Add(System.TimeSpan.FromSeconds(1));
        }

        //Do win here
    }
}
