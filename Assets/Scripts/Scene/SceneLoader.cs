using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingBar;
    [SerializeField] private GameObject animatedIcon;

    public System.Action OnSceneLoad;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    public void ReloadScene()
    {
        if (GameManager.instance)
        {
            GameManager.instance.ResumeGame();
            GameManager.instance.ResumeGameTime();
        }

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

    public void LoadScene(string name)
    {
        if (GameManager.instance)
        {
            GameManager.instance.ResumeGame();
            GameManager.instance.ResumeGameTime();
        }

        StartCoroutine(LoadSceneAsync(name));
    }

    private IEnumerator LoadSceneAsync(string name)
    {
        loadingScreen.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while(loadingBar.fillAmount < 1f)
        {
            loadingBar.fillAmount += Time.deltaTime / 3f;
            yield return null;
        }

        yield return new WaitUntil(() => op.progress >= 0.9f);

        loadingBar.transform.parent.gameObject.SetActive(false);
        animatedIcon.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        op.allowSceneActivation = true;
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
