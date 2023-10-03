using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{ 
    public static TutorialManager instance;

    [SerializeField] private GameObject dynamicPrompt;
    [SerializeField] private List<GameObject> dynamicPrompts = new();
    [SerializeField] private float dynamicPromptDuration;
    private Queue<string> dynamicPromptTextQueue = new();
    private Coroutine dynamicPromptCoroutine;

    public static bool isFirstTime = true;

    //StaticPrompt variables here
    //WARNING: CURRENTLY NO CHECKER FOR IF INDEX IS OVER/UNDER
    [SerializeField] private List<GameObject> StaticPrompts = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AddDynamicPrompt("you know what's funny?");
        AddDynamicPrompt("confidently says something wrong");
        AddDynamicPrompt("imagine");

        Scene currentScene = SceneManager.GetActiveScene();
        int buildIndex = currentScene.buildIndex;

        //StaticPrompts activation ritual
        //Checker if player wants prompts or not
        if (!isFirstTime)
        {
            return;
        }

        switch (buildIndex)
        {
            case 1:
                Instantiate(StaticPrompts[0]);
                AntigenManager.instance.OnAntigenPickup += EnablePromptOnAntigenPickup;
                RecruitManager.instance.OnRecruitSpawn += EnablePromptOnThresholdUpdate;
                UpgradeManager.instance.OnUpgradeScreen += EnablePromptOnUpgrade;
                UpgradeManager.instance.OnUltiGet += EnablePromptOnUltiGet;
                break;
            case 2:
                SymptomManager.instance.OnActivateSymptom += EnablePromptOnSymptom;
                break;
            default:
                break;
        }
        
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void EnablePromptOnAntigenPickup()
    {
        Instantiate(StaticPrompts[1]);
        AntigenManager.instance.OnAntigenPickup -= EnablePromptOnAntigenPickup;
    }

    public void EnablePromptOnThresholdUpdate()
    {
        Instantiate(StaticPrompts[2]);
        RecruitManager.instance.OnRecruitSpawn -= EnablePromptOnThresholdUpdate;
    }

    public void EnablePromptOnUpgrade()
    {
        Instantiate(StaticPrompts[3]);
        UpgradeManager.instance.OnUpgradeScreen -= EnablePromptOnUpgrade;
    }

    public void EnablePromptOnUltiGet()
    {
        Instantiate(StaticPrompts[4]);
        UpgradeManager.instance.OnUltiGet -= EnablePromptOnUltiGet;
    }

    public void EnablePromptOnSymptom()
    {
        Instantiate(StaticPrompts[5]);
        SymptomManager.instance.OnActivateSymptom -= EnablePromptOnSymptom;
    }

    public void AddDynamicPrompt(string text)
    {
        dynamicPromptTextQueue.Enqueue(text);

        StartCoroutine(DynamicPrompt());
    }

    private IEnumerator DynamicPrompt()
    {
        while (dynamicPromptTextQueue.Count > 0)
        {
            yield return new WaitWhile(() => dynamicPrompts.TrueForAll((prompt) => prompt.activeInHierarchy));
            var prompt = dynamicPrompts.Find((p) => !p.activeInHierarchy);
            ShowDynamicPrompt(prompt);
            yield return new WaitForSeconds(dynamicPromptDuration);
            dynamicPrompt.SetActive(false);
        }

        yield break;
    }

    private void ShowDynamicPrompt(in GameObject prompt)
    {
        prompt.SetActive(true);
        prompt.GetComponent<DynamicPrompt>().SetText(dynamicPromptTextQueue.Peek());
        dynamicPromptTextQueue.Dequeue();
    }
}
