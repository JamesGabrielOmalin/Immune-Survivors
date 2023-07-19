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
        AddDynamicPrompt("you know what's funny?");
        AddDynamicPrompt("confidently says something wrong");
        AddDynamicPrompt("imagine");
    }

    private void OnDestroy()
    {
        instance = null;
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
