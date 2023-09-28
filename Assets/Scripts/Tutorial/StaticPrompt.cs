using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StaticPrompt : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages = new();
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject closeButton;
    private int pageIndex = 0;

    public void OnEnable()
    {
        GameManager.instance.PauseGame();
        StartCoroutine(Disable());
    }

    public void OnDisable()
    {
        GameManager.instance.ResumeGame();
        StopAllCoroutines();
    }

    private IEnumerator Disable()
    {
        yield return new WaitUntil(() => Keyboard.current.escapeKey.IsPressed() && pageIndex >= pages.Count - 1);
        Destroy();
        Debug.Log("Pressed Esc");
    }

    public void NextPage()
    {
        if (pageIndex >= pages.Count)
            return;

        pages[pageIndex].SetActive(false);
        pageIndex++;
        pages[pageIndex].SetActive(true);

        nextButton.interactable = pageIndex < pages.Count-1;
        previousButton.interactable = true;

        if (!nextButton.interactable)
            closeButton.SetActive(true);
    }

    public void PreviousPage()
    {
        if (pageIndex <= 0)
            return;

        pages[pageIndex].SetActive(false);
        pageIndex--;
        pages[pageIndex].SetActive(true);

        nextButton.interactable = true;
        previousButton.interactable = pageIndex > 0;
        closeButton.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
