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
                //Replace into dynamic
                AntigenManager.instance.OnAntigenPickup += EnablePromptOnAntigenPickup;
                //Replace into dynamic
                RecruitManager.instance.OnRecruitSpawn += EnablePromptOnThresholdUpdate;
                UpgradeManager.instance.OnUpgradeScreen += EnablePromptOnUpgrade;
                //Replace into dynamic
                UpgradeManager.instance.OnUltiGet += EnablePromptOnUltiGet;
                for (int i = 0; i < 3; i++)
                    AntigenManager.instance.OnAntigenThresholdReached[(AntigenType)i] += EnablePromptOnAntigenThreshold;
                break;
            case 2:
                //Replace into dynamic
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
        //Instantiate(StaticPrompts[1]);
        AddDynamicPrompt("You just picked up an antigen!", StaticPrompts[1]);
        //AddDynamicPrompt("These little guys sometimes drop whenever bacteria dies");
        //AddDynamicPrompt("The color of the bacteria determines the color of the antigen");
        //AddDynamicPrompt("Pick up more so that B-Cells and T-Cells will spawn");

        AntigenManager.instance.OnAntigenPickup -= EnablePromptOnAntigenPickup;
    }

    public void EnablePromptOnThresholdUpdate()
    {
        //Instantiate(StaticPrompts[2]);
        AddDynamicPrompt("Backup has arrived!", StaticPrompts[2]);
        //AddDynamicPrompt("Somewhere, an ally has arrived, and will now help you");
        //AddDynamicPrompt("Follow the arrow to get to your ally and recruit them");
        //AddDynamicPrompt("Once you recruit them, you will become stronger!");

        RecruitManager.instance.OnRecruitSpawn -= EnablePromptOnThresholdUpdate;
    }

    public void EnablePromptOnUpgrade()
    {
        Instantiate(StaticPrompts[3]);
        UpgradeManager.instance.OnUpgradeScreen -= EnablePromptOnUpgrade;
    }

    public void EnablePromptOnUltiGet()
    {
        //Instantiate(StaticPrompts[4]);
        AddDynamicPrompt("You just unlocked your ultimate!");
        AddDynamicPrompt("You unlock your ultimate anytime you recruit 4 of your main unit");
        AddDynamicPrompt("Press Q to use your ultimate");

        UpgradeManager.instance.OnUltiGet -= EnablePromptOnUltiGet;
    }

    public void EnablePromptOnSymptom()
    {
        //Instantiate(StaticPrompts[5]);
        AddDynamicPrompt("A symptom has just occurred!");
        AddDynamicPrompt("This symptom right now is a Fever");
        AddDynamicPrompt("With fever, everyone gets damaged over time, while you increase speed");
        AddDynamicPrompt("Symptoms will be different each level");
        AddDynamicPrompt("Some levels might not even have symptoms");

        SymptomManager.instance.OnActivateSymptom -= EnablePromptOnSymptom;
    }

    public void EnablePromptOnAntigenThreshold()
    {
        AddDynamicPrompt("Upon gaining enough antigens, <color=yellow>Helper T Cells</color> and <color=yellow>B Cells</color> will start to spawn.");

        for (int i = 0; i < 3; i++)
            AntigenManager.instance.OnAntigenThresholdReached[(AntigenType)i] -= EnablePromptOnAntigenThreshold;
    }

    public void AddDynamicPrompt(string text)
    {
        dynamicPromptTextQueue.Enqueue(text);

        StartCoroutine(DynamicPrompt());
    }

    public void AddDynamicPrompt(string text, GameObject staticPrompt)
    {
        dynamicPromptTextQueue.Enqueue(text);

        StartCoroutine(DynamicPrompt(staticPrompt));
    }

    private IEnumerator DynamicPrompt()
    {
        while (dynamicPromptTextQueue.Count > 0)
        {
            yield return new WaitWhile(() => dynamicPrompt.activeInHierarchy);
            var prompt = dynamicPrompts.Find((p) => !p.activeInHierarchy);
            ShowDynamicPrompt();
            yield return new WaitForSeconds(dynamicPromptDuration);
            dynamicPrompt.SetActive(false);
        }

        yield break;
    }

    private IEnumerator DynamicPrompt(GameObject staticPrompt)
    {
        while (dynamicPromptTextQueue.Count > 0)
        {
            yield return new WaitWhile(() => dynamicPrompt.activeInHierarchy);
            var prompt = dynamicPrompts.Find((p) => !p.activeInHierarchy);
            ShowDynamicPrompt();
            yield return new WaitForSeconds(dynamicPromptDuration);
            dynamicPrompt.SetActive(false);
            Instantiate(staticPrompt);
        }

        yield break;
    }

    private void ShowDynamicPrompt()
    {
        dynamicPrompt.SetActive(true);
        dynamicPrompt.GetComponent<DynamicPrompt>().SetText(dynamicPromptTextQueue.Peek());
        dynamicPromptTextQueue.Dequeue();
    }
}
