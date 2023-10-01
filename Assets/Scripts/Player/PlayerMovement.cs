using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Animator> animators = new();
    [SerializeField] private CharacterController controller;
    [SerializeField] private Player player;
    [SerializeField] private PlayerInput input;
    [SerializeField] private List<SpriteRenderer> sprites = new();

    private Attribute moveSpeed;

    private readonly int ANIMATOR_ISMOVING = Animator.StringToHash("IsMoving");

    public Vector3 moveDir = Vector3.right;
    public Vector3 floorNormal = Vector3.up;
    public Vector3 inputDir = Vector3.right;
    public Vector3 lastInputDir = Vector3.right;

    // Start is called before the first frame update
    private void Start()
    {
        moveSpeed = player.GetActiveUnit().GetComponent<AttributeSet>().GetAttribute("Move Speed");
    }

    private void OnEnable()
    {
        controller.enabled = true;
    }

    private void OnDisable()
    {
        controller.enabled = false;
    }

    public void UpdateMoveSpeed()
    {
        //moveSpeed = player.GetActiveUnit().GetComponent<AttributeSet>().GetAttribute("Move Speed");
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 moveInput = input.MoveInput;

        bool hasInput = moveInput != Vector2.zero;

        foreach (var animator in animators)
        {
            if (animator.isActiveAndEnabled)
                animator.SetBool(ANIMATOR_ISMOVING, hasInput);
        }

        //else if (moveInput.x != 0f)
        //    sprite.flipX = moveInput.x < 0f;

        inputDir = new(moveInput.x, 0f, moveInput.y);
        moveDir = inputDir;

        //if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 1.3f))
        //{
        //    float distance = hit.distance - 1f;
        //    //controller.Move(new Vector3(0, distance, 0));
        //    moveDir.y += distance;
        //    floorNormal = hit.normal;
        //}
        //else
        //    floorNormal = Vector3.up;

        controller.Move(moveDir * (moveSpeed.Value * Time.deltaTime));
    
        if (controller.velocity.x != 0f)
        {
            foreach (var sprite in sprites)
            {
                if (sprite.enabled)
                    sprite.flipX = moveInput.x < 0f;
            }
        }

        if (hasInput)
            lastInputDir = inputDir;
    }
}
