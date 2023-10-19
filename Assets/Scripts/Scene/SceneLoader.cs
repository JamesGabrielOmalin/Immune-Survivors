using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Rendering;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingBar;
    [SerializeField] private GameObject animatedIcon;

    public System.Action OnSceneLoad;

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
        Time.timeScale = 1.0f;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void ReloadScene()
    {
        if (GameManager.instance)
        {
            GameManager.instance.ResumeGame();
            GameManager.instance.ResumeGameTime();
        }

        Time.timeScale = 1;
        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().name));
    }

    //private IEnumerator ReloadSceneAsync()
    //{
    //    string name = SceneManager.GetActiveScene().name;
    //    AsyncOperation op = SceneManager.UnloadSceneAsync(name);

    //    while (op.progress < 0.9f)
    //    {
    //        yield return null;
    //    }

    //    yield return LoadSceneAsync(name);
    //}

    public void LoadScene(int index)
    {
        if (GameManager.instance)
        {
            GameManager.instance.ResumeGame();
            GameManager.instance.ResumeGameTime();
        }

        Time.timeScale = 1;
        StartCoroutine(LoadSceneAsync(index));
    }

    public void LoadScene(string name)
    {
        if (GameManager.instance)
        {
            GameManager.instance.ResumeGame();
            GameManager.instance.ResumeGameTime();
        }

        Time.timeScale = 1;
        StartCoroutine(LoadSceneAsync(name));
    }

    private static readonly WaitForSecondsRealtime loadDelay = new(0.5f);

    private IEnumerator LoadSceneAsync(int index)
    {
        loadingScreen.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        float t = 0f;

        yield return null;

        while (t < 1f || loadingBar.fillAmount < 1f)
        {
            var delta = Time.unscaledDeltaTime * 0.25f;
            t += delta;
            loadingBar.fillAmount += delta;
            yield return null;
        }

        yield return new WaitUntil(() => op.progress >= 0.9f);
        Debug.Log("Done loading");

        loadingBar.transform.parent.gameObject.SetActive(false);
        animatedIcon.SetActive(true);

        yield return loadDelay;

        op.allowSceneActivation = true;
        Debug.Log("Activating scene");
    }

    private IEnumerator LoadSceneAsync(string name)
    {
        loadingScreen.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        float t = 0f;

        yield return null;

        while (t < 1f || loadingBar.fillAmount < 1f)
        {
            var delta = Time.unscaledDeltaTime * 0.25f;
            t += delta;
            loadingBar.fillAmount += delta;
            yield return null;
        }

        yield return new WaitUntil(() => op.progress >= 0.9f);
        Debug.Log("Done loading");

        loadingBar.transform.parent.gameObject.SetActive(false);
        animatedIcon.SetActive(true);

        yield return loadDelay;

        op.allowSceneActivation = true;
        Debug.Log("Activating scene");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
