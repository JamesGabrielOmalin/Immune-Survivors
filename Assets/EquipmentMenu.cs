using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentMenu : MonoBehaviour
{
    private Animator animator;

    private bool flag = false;

    [SerializeField] private TMP_Text label;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.TryGetComponent<Animator>(out animator))
        {

        }

        animator.keepAnimatorStateOnDisable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleEquipmentMenu()
    {
        flag = !flag;

        if (flag == true)
        {
            animator.SetTrigger("Show");
            label.text = "[E] HIDE EQUIPMENT";
            Debug.Log("Toggled equipment menu");

        }
        else
        {
            animator.SetTrigger("Hide");
            label.text = "[E] SHOW EQUIPMENT";

            Debug.Log("Untoggled equipment menu");


        }
    }

    public void ToggleEquipmentMenu(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.phase == UnityEngine.InputSystem.InputActionPhase.Started)
        {
            ToggleEquipmentMenu();
        }
    }
}
