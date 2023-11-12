using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Animator> animators = new();
    [SerializeField] private CharacterController controller;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Player player;
    [SerializeField] private PlayerInput input;
    [SerializeField] private List<SpriteRenderer> sprites = new();

    //private Attribute moveSpeed;
    private readonly Dictionary<PlayerUnitType, Attribute> moveSpeedAttributes = new();

    private readonly int ANIMATOR_ISMOVING = Animator.StringToHash("IsMoving");

    public Vector3 moveDir = Vector3.right;
    public Vector3 floorNormal = Vector3.up;
    public Vector3 inputDir = Vector3.right;
    public Vector3 lastInputDir = Vector3.right;

    public float moveSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        //moveSpeed = player.GetActiveUnit().GetComponent<AttributeSet>().GetAttribute("Move Speed");
        moveSpeedAttributes[PlayerUnitType.Neutrophil] = player.GetUnit(PlayerUnitType.Neutrophil).GetComponent<AttributeSet>().GetAttribute("Move Speed");
        moveSpeedAttributes[PlayerUnitType.Macrophage] = player.GetUnit(PlayerUnitType.Macrophage).GetComponent<AttributeSet>().GetAttribute("Move Speed");
        moveSpeedAttributes[PlayerUnitType.Dendritic] = player.GetUnit(PlayerUnitType.Dendritic).GetComponent<AttributeSet>().GetAttribute("Move Speed");

        moveSpeed = Mathf.Max(new float[]
        {   moveSpeedAttributes[PlayerUnitType.Neutrophil].Value,
            moveSpeedAttributes[PlayerUnitType.Macrophage].Value,
            moveSpeedAttributes[PlayerUnitType.Dendritic].Value }
        ); 
        StartCoroutine(UpdateMoveSpeed());
    }

    private void OnEnable()
    {
        //controller.enabled = true;
        rigidbody.WakeUp();
    }

    private void OnDisable()
    {
        //controller.enabled = false;
        rigidbody.Sleep();
    }

    private readonly WaitForSeconds wait = new(0.25f);

    public IEnumerator UpdateMoveSpeed()
    {
        while (this)
        {
            yield return wait;
            moveSpeed = Mathf.Max(new float[] 
            {   moveSpeedAttributes[PlayerUnitType.Neutrophil].Value, 
                moveSpeedAttributes[PlayerUnitType.Macrophage].Value, 
                moveSpeedAttributes[PlayerUnitType.Dendritic].Value }
            );
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
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

        if (!hasInput)
            return;

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

        //controller.Move(moveDir * (moveSpeed.Value * Time.deltaTime));

        // Get the greatest movement speed between the units

        var deltaPos = moveDir * (moveSpeed* Time.fixedDeltaTime);

        rigidbody.MovePosition(rigidbody.transform.position + deltaPos);

        foreach (var sprite in sprites)
        {
            if (sprite.enabled && hasInput)
                sprite.flipX = moveInput.x < 0f;
        }

        if (hasInput)
            lastInputDir = inputDir;
    }
}
