using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneLoader : MonoBehaviour
{
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
        StartCoroutine(LoadSceneAsync(name));
    }

    private IEnumerator LoadSceneAsync(string name)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while(op.progress < 0.9f)
        {
            // TODO: Loading screen here
            yield return null;
        }

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
