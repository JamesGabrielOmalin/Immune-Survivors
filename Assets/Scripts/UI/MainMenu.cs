using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private SceneLoader sceneLoader;
    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitGame()
    {
        if (sceneLoader == null)
        {
            Debug.Log(" Scene Loader NOT Found");
            return;
        }
        sceneLoader.ExitGame();
    }
}
