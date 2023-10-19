using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{ 
    public static TutorialManager instance;

    public GameObject dynamicPrompt;
    [SerializeField] private List<GameObject> dynamicPrompts = new();
    [SerializeField] private float dynamicPromptDuration;
    private Queue<string> dynamicPromptTextQueue = new();
    private Queue<GameObject> staticPromptQueue = new();
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
        AntigenManager.instance.OnAntigenPickup -= EnablePromptOnAntigenPickup;

        //Instantiate(StaticPrompts[1]);
        AddDynamicPrompt("You just picked up an antigen!", StaticPrompts[1]);
        //AddDynamicPrompt("These little guys sometimes drop whenever bacteria dies");
        //AddDynamicPrompt("The color of the bacteria determines the color of the antigen");
        //AddDynamicPrompt("Pick up more so that B-Cells and T-Cells will spawn");
    }

    public void EnablePromptOnThresholdUpdate()
    {
        RecruitManager.instance.OnRecruitSpawn -= EnablePromptOnThresholdUpdate;

        //Instantiate(StaticPrompts[2]);
        AddDynamicPrompt("Backup has arrived!", StaticPrompts[2]);
        //AddDynamicPrompt("Somewhere, an ally has arrived, and will now help you");
        //AddDynamicPrompt("Follow the arrow to get to your ally and recruit them");
        //AddDynamicPrompt("Once you recruit them, you will become stronger!");

        
    }

    public void EnablePromptOnUpgrade()
    {
        UpgradeManager.instance.OnUpgradeScreen -= EnablePromptOnUpgrade;

        Instantiate(StaticPrompts[3]);
    }

    public void EnablePromptOnUltiGet()
    {
        UpgradeManager.instance.OnUltiGet -= EnablePromptOnUltiGet;

        //Instantiate(StaticPrompts[4]);
        AddDynamicPrompt("You just unlocked your ultimate!");
        AddDynamicPrompt("You unlock your ultimate anytime you recruit 4 of your main unit");
        AddDynamicPrompt("Press <color=yellow>Q</color> to use your ultimate");
    }

    public void EnablePromptOnSymptom()
    {
        SymptomManager.instance.OnActivateSymptom -= EnablePromptOnSymptom;

        //Instantiate(StaticPrompts[5]);
        AddDynamicPrompt("A <color=yellow>symptom</color> has just occurred!");
        AddDynamicPrompt("This symptom right now is a <color=red>Fever</color>");
        AddDynamicPrompt("With <color=red>fever</color>, everyone gets damaged over time, while you increase speed");
        AddDynamicPrompt("<color=yellow>Symptoms</color> will be different each level");
        AddDynamicPrompt("Some levels might not even have <color=yellow>symptoms</color>");
    }

    public void EnablePromptOnAntigenThreshold()
    {
        for (int i = 0; i < 3; i++)
            AntigenManager.instance.OnAntigenThresholdReached[(AntigenType)i] -= EnablePromptOnAntigenThreshold;

        AddDynamicPrompt("Upon gaining enough antigens, <color=yellow>Helper T Cells</color> and <color=yellow>B Cells</color> will start to spawn.");
        AddDynamicPrompt("T Cells will help make you <color=blue>stronger</color> while B Cells make bacteria <color=red>weaker</color>");
        AddDynamicPrompt("However, these <color=blue>buffs</color> and <color=red>debuffs</color> only work against bacteria of the same color");
    }

    public void AddDynamicPrompt(string text)
    {
        dynamicPromptTextQueue.Enqueue(text);
        staticPromptQueue.Enqueue(null);

        if (dynamicPromptCoroutine == null)
            dynamicPromptCoroutine = StartCoroutine(DynamicPrompt());
    }

    public void AddDynamicPrompt(string text, GameObject staticPrompt)
    {
        dynamicPromptTextQueue.Enqueue(text);
        staticPromptQueue.Enqueue(staticPrompt);

        if (dynamicPromptCoroutine == null)
            dynamicPromptCoroutine = StartCoroutine(DynamicPrompt());
    }

    private IEnumerator DynamicPrompt()
    {
        while (dynamicPromptTextQueue.Count > 0)
        {
            ShowDynamicPrompt();
            yield return new WaitForSeconds(dynamicPromptDuration);
            dynamicPrompt.SetActive(false);
            ShowStaticPrompt();
        }

        dynamicPromptCoroutine = null;

        yield break;
    }

    private void ShowDynamicPrompt()
    {
        dynamicPrompt.SetActive(true);
        dynamicPrompt.GetComponent<DynamicPrompt>().SetText(dynamicPromptTextQueue.Peek());
        dynamicPromptTextQueue.Dequeue();
    }

    private void ShowStaticPrompt()
    {
        GameObject prompt = staticPromptQueue.Dequeue();

        if (!prompt)
            return;

        Instantiate(prompt);
    }
}
