using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public PlayerInputControls Controls { get; private set; }

    public Vector2 MoveInput => Controls.Movement.Move.ReadValue<Vector2>();

    private void Awake()
    {
        EnableControls();
    }

    private void OnEnable()
    {
        EnableControls();
    }

    private void OnDisable()
    {
        DisableControls();
    }
    
    private void EnableControls()
    {
        if (Controls == null)
            Controls = new();
        Controls.Enable();
    }

    private void DisableControls()
    {
        if (Controls == null)
            return;
        Controls.Disable();
    }
}
