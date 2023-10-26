using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifestyleManager : MonoBehaviour
{
    public static LifestyleManager instance;

    [SerializeField] private List<Effect> positiveLifestyles;
    [SerializeField] private List<Effect> negativeLifestyles;
    [SerializeField] private List<Effect> neutralLifestyles;
    private List<Effect> grantedLifestyles = new();

    [Header("UI")]
    [SerializeField] private LifestyleScreen lifestyleScreen;

    Coroutine LSCor;

    public System.Action OnActivateLifestyleScreen;

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
        LSCor = StartCoroutine(LifestyleCoroutine());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        instance = null;
    }

    public Effect[] GetRandomLifestyles()
    {
        Effect[] lifestyles = new Effect[3];

        lifestyles[0] = positiveLifestyles.GenerateRandom(1)[0];
        lifestyles[1] = negativeLifestyles.GenerateRandom(1)[0];
        lifestyles[2] = neutralLifestyles.GenerateRandom(1)[0];

        return lifestyles;
    }

    public void OpenLifestyleScreen()
    {
        AudioManager.instance.Play("UpgradePrompt", transform.position);
        GameManager.instance.PauseGameTime();
        GameManager.instance.HUD.SetActive(false);
        GameManager.instance.Player.GetComponent<Player>().EnableHUD(false);
        lifestyleScreen.SelectLifestyles();
        lifestyleScreen.gameObject.SetActive(true);
    }

    public void AddLifestyle(Effect effect)
    {
        grantedLifestyles.Add(effect);
    }

    private readonly WaitForSeconds wait = new(180.0f);

    private IEnumerator LifestyleCoroutine()
    {
        // Wait for 3 minutes
        yield return wait;

        // Wait in case upgrade screen is open
        yield return new WaitWhile(() => UpgradeManager.instance.IsUpgradeScreenOpen);

        OnActivateLifestyleScreen?.Invoke();
        OpenLifestyleScreen();
    }
}
