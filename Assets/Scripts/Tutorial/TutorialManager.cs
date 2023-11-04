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
    private Queue<string> dynamicPromptTitleQueue = new();

    private Queue<string> dynamicPromptTextQueue = new();
    private Queue<GameObject> staticPromptQueue = new();
    private Coroutine dynamicPromptCoroutine;

    public static bool isFirstTime = true;

    [SerializeField] Animator dynamicAnimator;

    //StaticPrompt variables here
    //WARNING: CURRENTLY NO CHECKER FOR IF INDEX IS OVER/UNDER
    [SerializeField] private List<GameObject> StaticPrompts = new List<GameObject>();

    public System.Action OnEnemyVisible;
    public System.Action OnPlayerHit;


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
        if(dynamicPrompt.TryGetComponent<Animator>(out dynamicAnimator))
        {
            Debug.Log("Animator Found");
        }
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
                EnablePromptOnIntro();
                //Replace into dynamic
                AntigenManager.instance.OnAntigenPickup += EnablePromptOnAntigenPickup;
                //Replace into dynamic
                RecruitManager.instance.OnRecruitSpawn += EnablePromptOnThresholdUpdate;
                UpgradeManager.instance.OnUpgradeScreen += EnablePromptOnUpgrade;
                //Replace into dynamic
                UpgradeManager.instance.OnUltiGet += EnablePromptOnUltiGet;
                LifestyleManager.instance.OnActivateLifestyleScreen += EnablePromptOnLifestyle;

                OnEnemyVisible += EnablePromptOnHUDTutorial;
                OnPlayerHit += EnablePromptOnPlayerHit;

                for (int i = 0; i < 3; i++)
                    AntigenManager.instance.OnAntigenThresholdReached[(AntigenType)i] += EnablePromptOnAntigenThreshold;
                break;
            case 2:
                //Replace into dynamic
                SymptomManager.instance.OnActivateSymptom += EnablePromptOnFeverSymptom;
                EnablePromptOnFeverIntro();
                break;
            case 3:
                SymptomManager.instance.OnActivateSymptom += EnablePromptOnCoughSymptom;
                EnablePromptOnCoughIntro();
                break;
            default:
                break;
        }
        
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void EnablePromptOnIntro()
    {   
        Instantiate(StaticPrompts[0]);
        //AddDynamicPrompt("These little guys sometimes drop whenever bacteria dies");
        //AddDynamicPrompt("The color of the bacteria determines the color of the antigen");
        //AddDynamicPrompt("Pick up more so that B-Cells and T-Cells will spawn");
        EnablePromptOnPlayerMovement();
    }

    public void EnablePromptOnPlayerMovement()
    {
        AddDynamicPrompt("BASIC CONTROLS", "Press <color=yellow>WASD</color> to move around and <color=yellow>[SPACEBAR]</color> to use your mobility ability");
        AddDynamicPrompt("INVADING BACTERIA", " As you know, your body is under attack, and not for long <b>bacteria</b> will start to invade.");

    }

    public void EnablePromptOnHUDTutorial()
    {
        //Instantiate(StaticPrompts[1]);
        OnEnemyVisible-= EnablePromptOnHUDTutorial;
    }
    public void EnablePromptOnAntigenPickup()
    {
        AntigenManager.instance.OnAntigenPickup -= EnablePromptOnAntigenPickup;

        //Instantiate(StaticPrompts[1]);
        AddDynamicPrompt(" ANTIGENS", "You just picked up an <color=yellow>antigen</color>!", StaticPrompts[1]);
        //AddDynamicPrompt("These little guys sometimes drop whenever bacteria dies");
        //AddDynamicPrompt("The color of the bacteria determines the color of the antigen");
        //AddDynamicPrompt("Pick up more so that B-Cells and T-Cells will spawn");
    }

    public void EnablePromptOnThresholdUpdate()
    {
        RecruitManager.instance.OnRecruitSpawn -= EnablePromptOnThresholdUpdate;

        //Instantiate(StaticPrompts[2]);
        AddDynamicPrompt("BACKUP RECRUITABLES","Backup has arrived!", StaticPrompts[2]);
        //AddDynamicPrompt("Somewhere, an ally has arrived, and will now help you");
        //AddDynamicPrompt("Follow the arrow to get to your ally and recruit them");
        //AddDynamicPrompt("Once you recruit them, you will become stronger!");
    }

    public void EnablePromptOnPlayerHit()
    {
        TutorialManager.instance.OnPlayerHit -= EnablePromptOnPlayerHit;

        //Instantiate(StaticPrompts[2]);
        AddDynamicPrompt("TAKING DAMAGE", " Watch out! Getting hit by enemies will deplete your <color=yellow>HP Bar (BOTTOM HUD)</color>.");
        AddDynamicPrompt("RESTORING HP", " While your HP can <color=green>regenerate over time</color>. You can also <color=yellow>replenish your HP by recruiting other units</color>.");

        //AddDynamicPrompt("Somewhere, an ally has arrived, and will now help you");
        //AddDynamicPrompt("Follow the arrow to get to your ally and recruit them");
        //AddDynamicPrompt("Once you recruit them, you will become stronger!");
    }

    public void EnablePromptOnUpgrade()
    {
        UpgradeManager.instance.OnUpgradeScreen -= EnablePromptOnUpgrade;

        Instantiate(StaticPrompts[3]);
    }
    public void EnablePromptOnLifestyle()
    {
        LifestyleManager.instance.OnActivateLifestyleScreen -= EnablePromptOnLifestyle;
        Instantiate(StaticPrompts[8]);
    }

    public void EnablePromptOnUltiGet()
    {
        UpgradeManager.instance.OnUltiGet -= EnablePromptOnUltiGet;

        //Instantiate(StaticPrompts[4]);
        AddDynamicPrompt("ULTIMATE", "You just unlocked your ultimate!");
        AddDynamicPrompt("ULTIMATE","You unlock your ultimate anytime you recruit 4 of your main unit");
        AddDynamicPrompt("ULTIMATE","Press <color=yellow>[Q]</color> to use your ultimate");
    }

    public void EnablePromptOnFeverIntro()
    {

        //Instantiate(StaticPrompts[5]);
        AddDynamicPrompt("INTRODUCTION", "A fever is our body's defense mechanism to various illnesses and infection", 7);
        AddDynamicPrompt("INTRODUCTION", "it's when your body's temperature rises to fight off invaders like bacteria", 7);
        AddDynamicPrompt("INTRODUCTION", "Fevers can range from 37.7 degrees Celsius to a life-threatening 41.6 degrees Celsius!", 7);
        AddDynamicPrompt("INTRODUCTION", "Bear in mind, the average temperature of an adult human is 37 degrees Celsius. That's a 4.6 degree difference!", 7);
        AddDynamicPrompt("INTRODUCTION", "Your immune cells have your back, but do not be afraid of seeking medical attention when you can't handle the fever", 7);

    }
    public void EnablePromptOnFeverSymptom()
    {
        SymptomManager.instance.OnActivateSymptom -= EnablePromptOnFeverSymptom;

        //Instantiate(StaticPrompts[5]);
        AddDynamicPrompt("FEVER SYMPTOM", "This symptom right now is a <color=red>Fever</color>", 7);
        AddDynamicPrompt("FEVER SYMPTOM", "<color=red>Fever</color> boosts your immune cells' speed and deals damage over time (DoT) to bacteria", 7);
        AddDynamicPrompt("FEVER SYMPTOM", "But be careful! Extreme fever can harm your immune cells if it gets too high, causing them to weaken and get damaged.", 8);
    }

    public void EnablePromptOnCoughIntro()
    {
        AddDynamicPrompt("INTRODUCTION", "Coughing is our body's way of clearing our throat of various irritants",7);
        AddDynamicPrompt("INTRODUCTION", "We cough whenever our body detects foreign entities, particularly germs and particles, on our throat", 7);
        AddDynamicPrompt("INTRODUCTION", "Coughing on occasion is normal. However, there is cause for concern when the coughs become more serious, as whatever is on the throat refuses to leave", 10);
        AddDynamicPrompt("INTRODUCTION", "Our immune cells are tough, but we can help our immune cells by drinking medicine and getting good sleep.", 8);
    }

    public void EnablePromptOnCoughSymptom()
    {
        SymptomManager.instance.OnActivateSymptom -= EnablePromptOnCoughSymptom;

        AddDynamicPrompt("COUGH SYMPTOM", "The symptom right now is <color=red>Cough</color>",7);
        AddDynamicPrompt("COUGH SYMPTOM", "<color=red>Cough</color> pushes around the bacteria based on a random direction");
        AddDynamicPrompt("COUGH SYMPTOM", "Watch for the warnings and steer clear from the bacteria!");
    }

    public void EnablePromptOnAntigenThreshold()
    {
        for (int i = 0; i < 3; i++)
            AntigenManager.instance.OnAntigenThresholdReached[(AntigenType)i] -= EnablePromptOnAntigenThreshold;

        AddDynamicPrompt("ACTIVATING THE ADAPTIVE UNITS", "Upon gaining enough antigens, <color=yellow>Helper T Cells</color> and <color=yellow>B Cells</color> will start to spawn.", StaticPrompts[7]);
        //AddDynamicPrompt("ACTIVATING THE ADAPTIVE UNITS", "T Cells will help make you <color=blue>stronger</color> while B Cells make bacteria <color=red>weaker</color>");
        //AddDynamicPrompt("ACTIVATING THE ADAPTIVE UNITS", "However, these <color=blue>buffs</color> and <color=red>debuffs</color> only work against bacteria of the same color");
    }
    public void AddDynamicPrompt(string title, string text, float duration)
    {
        dynamicPromptTitleQueue.Enqueue(title);
        dynamicPromptTextQueue.Enqueue(text);
        staticPromptQueue.Enqueue(null);

        if (dynamicPromptCoroutine == null)
            dynamicPromptCoroutine = StartCoroutine(DynamicPrompt(duration));
    }
    public void AddDynamicPrompt(string title, string text)
    {
        dynamicPromptTitleQueue.Enqueue(title);
        dynamicPromptTextQueue.Enqueue(text);
        staticPromptQueue.Enqueue(null);

        if (dynamicPromptCoroutine == null)
            dynamicPromptCoroutine = StartCoroutine(DynamicPrompt());
    }

    public void CreateStaticPrompt(GameObject staticPrompt)
    {
        staticPromptQueue.Enqueue(staticPrompt);
        ShowStaticPrompt();

    }
    public void AddDynamicPrompt(string title, string text, GameObject staticPrompt, float duration)
    {
        dynamicPromptTitleQueue.Enqueue(title);
        dynamicPromptTextQueue.Enqueue(text);
        staticPromptQueue.Enqueue(staticPrompt);

        if (dynamicPromptCoroutine == null)
            dynamicPromptCoroutine = StartCoroutine(DynamicPrompt(duration));
    }

    public void AddDynamicPrompt(string title, string text, GameObject staticPrompt)
    {
        dynamicPromptTitleQueue.Enqueue(title);
        dynamicPromptTextQueue.Enqueue(text);
        staticPromptQueue.Enqueue(staticPrompt);

        if (dynamicPromptCoroutine == null)
            dynamicPromptCoroutine = StartCoroutine(DynamicPrompt());
    }

    private IEnumerator DynamicPrompt()
    {
        while (dynamicPromptTextQueue.Count > 0 && dynamicPromptTitleQueue.Count > 0)
        {
            
            ShowDynamicPrompt();
            dynamicAnimator.SetBool("toShow", true);
            yield return new WaitForSeconds(dynamicPromptDuration);
            dynamicAnimator.SetBool("toShow", false);
            yield return new WaitForSeconds(0.3f);


            //dynamicPrompt.SetActive(false);
            ShowStaticPrompt();
            
           
        }

        dynamicPromptCoroutine = null;

        yield break;
    }

    private IEnumerator DynamicPrompt(float duration)
    {
        while (dynamicPromptTextQueue.Count > 0 && dynamicPromptTitleQueue.Count > 0)
        {

            ShowDynamicPrompt();
            dynamicAnimator.SetBool("toShow", true);
            yield return new WaitForSeconds(dynamicPromptDuration);
            dynamicAnimator.SetBool("toShow", false);
            yield return new WaitForSeconds(0.3f);


            //dynamicPrompt.SetActive(false);
            ShowStaticPrompt();

        }

        dynamicPromptCoroutine = null;

        yield break;
    }

    private void ShowDynamicPrompt()
    {
        dynamicPrompt.SetActive(true);
        dynamicPrompt.GetComponent<DynamicPrompt>().SetText(dynamicPromptTitleQueue.Peek(),dynamicPromptTextQueue.Peek());
        dynamicPromptTitleQueue.Dequeue();
        dynamicPromptTextQueue.Dequeue();
    }

    private void ShowStaticPrompt()
    {
        GameObject prompt = staticPromptQueue.Dequeue();

        if (!prompt)
            return;

        GameObject obj = Instantiate(prompt);
    }
}
