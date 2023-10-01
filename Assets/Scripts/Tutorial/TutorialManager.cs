using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{ 
    public static TutorialManager instance;

    [SerializeField] private GameObject dynamicPrompt;
    [SerializeField] private float dynamicPromptDuration;
    private Queue<string> dynamicPromptTextQueue = new();
    private Coroutine dynamicPromptCoroutine;

    public static bool isFirstTime;

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
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AddDynamicPrompt("you know what's funny?");
        AddDynamicPrompt("confidently says something wrong");
        AddDynamicPrompt("imagine");

        //StaticPrompts activation ritual
        
        //Checker if player wants prompts or not
        if (!isFirstTime)
        {
            return;
        }

        Instantiate(StaticPrompts[0]);
        AntigenManager.instance.OnAntigenPickup += EnablePromptOnAntigenPickup;
        RecruitManager.instance.OnRecruitSpawn += EnablePromptOnThresholdUpdate;
        UpgradeManager.instance.OnUpgradeScreen += EnablePromptOnUpgrade;
        UpgradeManager.instance.OnUltiGet += EnablePromptOnUltiGet;
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

    public void AddDynamicPrompt(string text)
    {
        dynamicPromptTextQueue.Enqueue(text);

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
        }

        dynamicPromptCoroutine = null;
        yield break;
    }

    public void ShowDynamicPrompt()
    {
        dynamicPrompt.SetActive(true);
        dynamicPrompt.GetComponent<DynamicPrompt>().SetText(dynamicPromptTextQueue.Peek());
        dynamicPromptTextQueue.Dequeue();
    }
}
