using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AttributeSet attributes;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private SpriteRenderer sprite;

    private GameObject target;
    private Attribute moveSpeed;
    private Vector3 moveDir;

    // Start is called before the first frame update
    private void Start()
    {
        moveSpeed = attributes.GetAttribute("Move Speed");
        target = GameManager.instance.Player;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (enemy.IsStunned || Vector3.Distance(transform.position, target.transform.position) <= 2f)
            return;
        moveDir = (target.transform.position - transform.position).normalized;

        rigidBody.MovePosition(transform.position + moveDir * (moveSpeed.Value * Time.fixedDeltaTime));

        sprite.flipX = moveDir.x > 0;
    }
}
