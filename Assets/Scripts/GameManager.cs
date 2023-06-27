using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [field: SerializeField] public GameObject Player { get; private set; }

    public System.TimeSpan GameTime { get; private set; }

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
    }
    public void UnpauseGameTime()
    {
        Time.timeScale = 1f;
    }

    private IEnumerator GameTimer()
    {
        WaitForSeconds wait = new(1f);
        
        while (this)
        {
            yield return wait;
            GameTime.Add(System.TimeSpan.FromSeconds(1));
        }
    }
}
