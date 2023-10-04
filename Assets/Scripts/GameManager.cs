using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [field: SerializeField] public GameObject Player { get; private set; }
    [field: SerializeField] public GameObject HUD { get; private set; }

    public System.TimeSpan GameTime { get; private set; }
    public string GameTimeDisplay => GameTime.ToString(@"mm\:ss");
    public float TimeToWin;
    //public TMP_Text Timer;

    public bool GameTimePaused { get; private set; }

    public bool GamePaused { get; private set; }

    public System.Action OnGamePaused;
    public System.Action OnGameResumed;

    public System.Action OnGameWin;
    public System.Action OnGameLose;

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
        StartCoroutine(LastMinute());

        if (Player)
        {
            var input = Player.GetComponent<PlayerInput>();
            OnGamePaused += input.DisableControls;
            OnGameResumed += input.EnableControls;
        }
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
    private WaitForSeconds wait = new(1f);
    private IEnumerator GameTimer()
    {
        
        while (GameTime.TotalSeconds < TimeToWin)
        {
            yield return wait;
            GameTime = GameTime.Add(System.TimeSpan.FromSeconds(1d));

            //Text can only go to 60 minutes iirc. Might have to refactor if we're planning to add an infinite mode.
            //Timer.text = GameTime.ToString(@"mm\:ss");
        }

        //Do win here
        Player.SetActive(false); HUD.SetActive(false);
        OnGameWin?.Invoke();
    }

    // Not literal last minute, just 8 minutes lmao
    private readonly WaitForSeconds lastMinuteTimer = new(480);
    public System.Action OnLastMinuteReached;
    public bool LastMinuteReached { get; private set; }  = false;

    private IEnumerator LastMinute()
    {
        yield return lastMinuteTimer;
        OnLastMinuteReached?.Invoke();
        LastMinuteReached = true;
    }
}
